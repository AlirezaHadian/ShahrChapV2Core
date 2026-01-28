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
        public OrderService(ShahrChapContext context)
        {
            _context = context;
        }
        #region Order Status
        public List<OrderStatus> GetOrderStatuses()
        {
            return _context.OrderStatuses.OrderBy(o=> o.SortOrder).ToList();
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

        #endregion
    }
}
