using Microsoft.AspNetCore.Mvc;
using ShahrChap.Core.Services;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Order;

namespace ShahrChap.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        public OrderController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }
        public IActionResult Success(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null) return NotFound();

            if (!HasAccessToOrder(order)) return Forbid();

            return View(order);
        }

        public IActionResult Failed()
        {
            ViewBag.ErrorMessage = TempData["PaymentError"] as string ?? "پرداخت شما ناموفق بود. لطفاً دوباره تلاش کنید.";

            return View();
        }

        private bool HasAccessToOrder(Order order)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                int userId = _userService.GetUserIdWithUserName(User.Identity.Name);
                return order.UserId == userId;
            }

            var token = Request.Cookies["cart-token"];
            return !string.IsNullOrEmpty(token) && token == order.CheckoutToken;
        }
    }
}
