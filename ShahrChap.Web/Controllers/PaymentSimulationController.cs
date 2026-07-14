using Microsoft.AspNetCore.Mvc;
using ShahrChap.Core.DTOs.Order;
using ShahrChap.Core.DTOs.Payment;
using ShahrChap.Core.Services.Interfaces;

namespace ShahrChap.Web.Controllers
{
    public class PaymentSimulationController : Controller
    {
        private readonly ICartService _cartService;
        public PaymentSimulationController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpGet]
        public IActionResult Gateway(int cartId, long amount)
        {
            var vm = new PaymentGatewayViewModel { CartId = cartId, Amount = amount };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Callback(int cartId, bool isSuccessful)
        {
            var payment = new PaymentResultDto
            {
                IsSuccessful = isSuccessful,
                Authority = Guid.NewGuid().ToString("N"),
                RefId = isSuccessful ? new Random().Next(100000, 999999).ToString() : null
            };

            if (!isSuccessful)
                return RedirectToAction("Failed", "Order");

            try
            {
                var order = _cartService.ConvertCartToOrder(cartId, payment);
                return RedirectToAction("Success", "Order", new { orderId = order.OrderId });
            }
            catch (Exception ex)
            {
                TempData["PaymentError"] = "خطا در ثبت سفارش: " + ex.Message;
                return RedirectToAction("Failed", "Order");
            }
        }
    }
}
