using ShahrChap.DataLayer.Entities.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.Services.Interfaces
{
    public interface IOrderService
    {
        #region OrderStatus
        List<OrderStatus> GetOrderStatuses();
        int CreateOrderStatus(OrderStatus status);
        int GetLastSortOrder();
        #endregion
    }
}
