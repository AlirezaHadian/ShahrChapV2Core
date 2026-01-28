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
        #endregion
    }
}
