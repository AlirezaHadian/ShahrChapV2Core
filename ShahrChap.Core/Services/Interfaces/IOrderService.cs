using Microsoft.AspNetCore.Http;
using ShahrChap.Core.DTOs.Order;
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
        void UpdateSortOrder(List<int> ids);
        OrderStatus GetOrderStatusById(int orderStatusId);
        void UpdateOrderStatus(OrderStatus orderStatus);
        void DeleteOrderStatus(int orderStatusId);
        OrderStatus GetFirstOrderStatus();
        #endregion
        #region Order
        Task<int> CreateOrderAsync(string userName, CreateOrderDetailDto orderDto);
        //int CreateOrder(string userName, List<CreateOrderDetailDto> details)
        //List<OrderFile> SaveOrderFilesTemp(List<IFormFile> files);

        #endregion
    }
}
