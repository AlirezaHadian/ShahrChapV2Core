using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShahrChap.Core.Services.Interfaces;

namespace ShahrChap.Web.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IUserService _userService;
        public CheckoutController(ICartService cartService, IUserService userService)
        {
            _cartService = cartService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cartDetails = _cartService.ShowCart(User.Identity.Name);

            if (cartDetails.Cart == null || !cartDetails.Items.Any())
                return RedirectToAction("Index", "Cart");

            if (cartDetails.Cart.SelectedAddressId == null)
            {
                TempData["CheckoutError"] = "ابتدا یک آدرس ارسال انتخاب کنید.";
                return RedirectToAction("Index", "Cart");
            }

            return View(cartDetails);
        }

        [HttpPost]
        public IActionResult GoToGateway()
        {
            var cart = _cartService.GetUserActiveCart(User.Identity.Name);
            if (cart == null || !cart.CartItems.Any())
                return RedirectToAction("Index", "Cart");

            long amount = cart.CartItems.Sum(c => c.CalculatedPrice);
            return RedirectToAction("Gateway", "PaymentSimulation", new { cartId = cart.CartID, amount });
        }
    }
}
