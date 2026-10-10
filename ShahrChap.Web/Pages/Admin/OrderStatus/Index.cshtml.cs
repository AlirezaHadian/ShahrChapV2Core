using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Security;
using ShahrChap.Core.Services.Interfaces;

namespace ShahrChap.Web.Pages.Admin.OrderStatus
{
    [PermissionChecker(1023)]
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

        public IActionResult OnPostUpdateOrder([FromBody] List<int> ids)
        {
            if (ids == null) return new JsonResult(new { success = false });

            _orderService.UpdateSortOrder(ids);    
            return new JsonResult(new { success = true });
        }
    }
}
