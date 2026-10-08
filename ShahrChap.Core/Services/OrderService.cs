using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShahrChap.Core.DTOs.Order;
using ShahrChap.Core.Generators;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Context;
using ShahrChap.DataLayer.Entities.Order;
using ShahrChap.DataLayer.Entities.Product;
using ShahrChap.DataLayer.Entities.User;
using ShahrChap.DataLayer.Enums;
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
            orderStatus.IsDeleted = true;
            UpdateOrderStatus(orderStatus);
        }
        public OrderStatus GetFirstOrderStatus()
        {
            int firstOrderSort = _context.OrderStatuses.Min(o => o.SortOrder);
            return _context.OrderStatuses.FirstOrDefault(o => o.SortOrder == firstOrderSort);
        }
        #endregion
        #region Order
        public Order GetOrderById(int orderId)
        {
            return _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.OrderDetailServices)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Files)
                .Include(o => o.Status)
                .FirstOrDefault(o => o.OrderId == orderId);
        }
        private async Task HandleOrderFiles(int detailId, List<IFormFile> files, bool isTemp = true, string? subDirectory = null)
        {
            if (files == null || !files.Any())
                return;

            string basePath = isTemp ? "Temp" : "Orders";
            string subPath = subDirectory ?? (isTemp ? $"Temp_{detailId}" : $"Order_{detailId}");
            string targetDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads", basePath, subPath);

            if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                    continue;

                string fileName = NameGenerator.GenerateUniqCode() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(targetDir, fileName);

                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                catch (IOException ex)
                {
                    throw new Exception($"Failed to save file {fileName}: {ex.Message}", ex);
                }

                await _context.OrderFiles.AddAsync(new OrderFile
                {
                    DetailId = detailId,
                    FileName = fileName,
                    OriginalFileName = file.FileName,
                    IsTemp = isTemp,
                    UploadDate = DateTime.Now
                });
            }
            //await _context.SaveChangesAsync();
        }
        public async Task AddServicesToOrderDetail(int orderDetailId, List<int> servicesId, int productId, string combination)
        {
            for(int i=0; i<servicesId.Count; i++)
            {
                Service service = _productService.GetServiceById(servicesId[i]);
                int productPriceId = _productService.GetProductPriceId(productId, combination);
                decimal servicePrice = _productService.GetServicePriceForShowProduct(productPriceId, service.ServiceId);
                OrderDetailService orderDetailService = new OrderDetailService()
                {
                    OrderDetailID = orderDetailId,
                    ServiceID = service.ServiceId,
                    ServiceTitle = service.ServiceTitle,
                    ServicePrice = (long)servicePrice
                };
                _context.OrderDetailServices.Add(orderDetailService);
            }
            _context.SaveChanges();
        }
        #endregion
        #region Status
        public int GetInitialStatusId()
        {
            int? statusId = _context.OrderStatuses
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.SortOrder)
                .Select(s => (int?)s.StatusId)
                .FirstOrDefault();

            if (statusId == null)
                throw new InvalidOperationException("هیچ وضعیت اولیه‌ای برای سفارش تعریف نشده است.");

            return statusId.Value;
        }

        #endregion
    }
}