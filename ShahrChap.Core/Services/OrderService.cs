using ShahrChap.Core.DTOs.Order;
using ShahrChap.Core.Generators;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Context;
using ShahrChap.DataLayer.Entities.Order;
using ShahrChap.DataLayer.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.Services
{
    public class OrderService : IOrderService
    {
        private ShahrChapContext _context;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        public OrderService(ShahrChapContext context, IProductService productService, IUserService userService)
        {
            _context = context;
            _productService = productService;
            _userService = userService;
        }
        #region Order Status
        public List<OrderStatus> GetOrderStatuses()
        {
            return _context.OrderStatuses.OrderBy(o => o.SortOrder).ToList();
        }
        public int CreateOrderStatus(OrderStatus status)
        {
            _context.OrderStatuses.Add(status);
            _context.SaveChanges();
            return status.StatusId;
        }

        public int GetLastSortOrder()
        {
            return _context.OrderStatuses.Max(o => o.SortOrder);
        }

        public void UpdateSortOrder(List<int> ids)
        {
            var allStatuses = GetOrderStatuses();

            for (int i = 0; i < ids.Count; i++)
            {
                var status = allStatuses.FirstOrDefault(s => s.StatusId == ids[i]);
                if (status != null)
                {
                    status.SortOrder = i + 1;
                }
            }

            _context.SaveChanges();
        }

        public OrderStatus GetOrderStatusById(int orderStatusId)
        {
            return _context.OrderStatuses.Find(orderStatusId);
        }

        public void UpdateOrderStatus(OrderStatus orderStatus)
        {
            _context.OrderStatuses.Update(orderStatus);
            _context.SaveChanges();
        }

        public void DeleteOrderStatus(int orderStatusId)
        {
            OrderStatus orderStatus = GetOrderStatusById(orderStatusId);
            orderStatus.IsDelete = true;
            UpdateOrderStatus(orderStatus);
        }
        public OrderStatus GetFirstOrderStatus()
        {
            int firstOrderSort = _context.OrderStatuses.Min(o => o.SortOrder);
            return _context.OrderStatuses.FirstOrDefault(o => o.SortOrder == firstOrderSort);
        }
        #endregion
        #region Order
        public async Task<int> CreateOrderAsync(string userName, CreateOrderDetailDto orderDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int userId = _userService.GetUserIdWithUserName(userName);

                string services = string.Join("-",
orderDto.Services.Select(item =>
    _productService.GetServiceById(item).ServiceTitle
));

                Order order = _context.Orders
                    .FirstOrDefault(o => o.UserId == userId && !o.IsFinally);

                //long calculatedPrice = _productService.CalculatePrice(orderDto.ProductId);
                long calculatedPrice = 0;

                if (order == null)
                {
                    // ۱. ایجاد شیء اصلی سفارش (Order)
                    order = new Order
                    {
                        UserId = userId,
                        //OrderStatusId = GetFirstOrderStatus().StatusId,
                        CreateDate = DateTime.Now,
                        TotalPrice = calculatedPrice,
                        FinalPrice = calculatedPrice,
                        IsFinally = false
                    };
                    await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync();

                    // ۲. ایجاد جزئیات سفارش (OrderDetail)
                    var detail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        OrderDetailTitle = orderDto.OrderTitle,
                        ProductId = orderDto.ProductId,
                        ProductTitle = orderDto.ProductTitle,
                        FeaturesCombination = orderDto.FeaturesCombination,
                        Services = services,
                        Price = (int)calculatedPrice
                    };
                    await _context.OrderDetails.AddAsync(detail);
                    await _context.SaveChangesAsync();

                    // ۳. مدیریت فایل‌ها: انتقال از Temp به Orders
                    string tempDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads/Temp/Temp_" + detail.DetailId.ToString());
                    //string targetDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads/Orders/Order_"+ detail.DetailId.ToString());

                    if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);

                    foreach (var file in orderDto.Files)
                    {
                        string fileName = NameGenerator.GenerateUniqCode() + Path.GetExtension(file.FileName);
                        string filePath = Path.Combine(tempDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        await _context.OrderFiles.AddAsync(new OrderFile
                        {
                            DetailId = detail.DetailId,
                            FileName = fileName
                        });
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return order.OrderId;
                }
                else
                {
                    order.TotalPrice += calculatedPrice;
                    order.FinalPrice += calculatedPrice;

                    var detail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = orderDto.ProductId,
                        ProductTitle = orderDto.ProductTitle,
                        FeaturesCombination = orderDto.FeaturesCombination,
                        Services = services,
                        Price = (int)calculatedPrice
                    };
                    await _context.OrderDetails.AddAsync(detail);
                    await _context.SaveChangesAsync();

                    string tempDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads/Temp/Temp_" + detail.DetailId.ToString());

                    if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);

                    foreach (var file in orderDto.Files)
                    {
                        string fileName = NameGenerator.GenerateUniqCode() + Path.GetExtension(file.FileName);
                        string filePath = Path.Combine(tempDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        await _context.OrderFiles.AddAsync(new OrderFile
                        {
                            DetailId = detail.DetailId,
                            FileName = fileName
                        });

                    }
                    await transaction.CommitAsync();
                    return order.OrderId;
                }
            }
            catch (Exception)
            {
                // در صورت بروز هرگونه خطا، تمام تغییرات دیتابیس برمی‌گردد
                await transaction.RollbackAsync();

                // اختیاری: پاک کردن فایل‌های فیزیکی که احتمالا در این پوشه کپی شده‌اند
                // DeleteDirectory(targetDir); 

                return 0;
            }
        }
        #endregion 
    }
}
