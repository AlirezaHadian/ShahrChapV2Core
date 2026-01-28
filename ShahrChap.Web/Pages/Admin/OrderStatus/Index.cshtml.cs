using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Services.Interfaces;

namespace ShahrChap.Web.Pages.Admin.OrderStatus
{
    public class IndexModel : PageModel
    {
        private IOrderService _orderService;
        public IndexModel(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public List<DataLayer.Entities.Order.OrderStatus> OrderStatuses { get; set; }
        public void OnGet()
        {
            OrderStatuses = _orderService.GetOrderStatuses();
        }
    }
}
