using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Services.Interfaces;

namespace ShahrChap.Web.Pages.Admin.OrderStatus
{
    public class DeleteOrderStatusModel : PageModel
    {
        private IOrderService _orderService;
        public DeleteOrderStatusModel(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [BindProperty]
        public DataLayer.Entities.Order.OrderStatus OrderStatus { get; set; }
        public void OnGet(int id)
        {
            OrderStatus = _orderService.GetOrderStatusById(id);
        }
        public IActionResult OnPost()
        {
            _orderService.DeleteOrderStatus(OrderStatus.StatusId);
            return RedirectToPage("Index");
        }
    }
}
