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
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var cartDetails = _cartService.ShowCart(User.Identity.Name);
            if (cartDetails.Cart == null || !cartDetails.Items.Any())
                return RedirectToAction("Index", "Cart");

            return View(cartDetails);
        }
        [Authorize]
        [HttpPost]
        public IActionResult GoToGateway(int selectedAddressId)
        {
            string userName = User.Identity.IsAuthenticated ? User.Identity.Name : null;
            string token = Request.Cookies["cart-token"];

            var cart = _cartService.GetUserActiveCart(userName, token);
            if (cart == null || !cart.CartItems.Any())
                return RedirectToAction("Index", "Cart");

            long amount = cart.CartItems.Sum(c => c.CalculatedPrice);
            return RedirectToAction("Gateway", "PaymentSimulation", new { cartId = cart.CartID, amount });
        }
    }
}
