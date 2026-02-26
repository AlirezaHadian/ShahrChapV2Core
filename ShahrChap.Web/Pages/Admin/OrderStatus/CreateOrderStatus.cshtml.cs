using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Services;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Order;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Pages.Admin.OrderStatus
{
    public class CreateOrderStatusModel : PageModel
    {
        private IOrderService _orderSrevice;
        public CreateOrderStatusModel(IOrderService orderService)
        {
            _orderSrevice = orderService;
        }
        [BindProperty]
        public DataLayer.Entities.Order.OrderStatus OrderStatus { get; set; }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            OrderStatus.SortOrder = _orderSrevice.GetLastSortOrder() + 1;
            OrderStatus.IsDeleted = false;
            _orderSrevice.CreateOrderStatus(OrderStatus);

            return RedirectToPage("Index");
        }
    }
}
