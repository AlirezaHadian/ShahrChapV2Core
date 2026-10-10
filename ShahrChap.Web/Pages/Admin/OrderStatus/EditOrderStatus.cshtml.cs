using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Security;
using ShahrChap.Core.Services;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;
using System.CodeDom;

namespace ShahrChap.Web.Pages.Admin.OrderStatus
{
    [PermissionChecker(1026)]
    public class EditOrderStatusModel : PageModel
    {
        private IOrderService _orderService;
        public EditOrderStatusModel(IOrderService orderService)
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
            if (!ModelState.IsValid)
                return Page();

            _orderService.UpdateOrderStatus(OrderStatus);
            return RedirectToPage("Index");
        }
    }
}
