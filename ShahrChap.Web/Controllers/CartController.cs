using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.DependencyResolver;
using ShahrChap.Core.DTOs.Cart;
using ShahrChap.Core.Services.Interfaces;
using System.Security.Claims;

namespace ShahrChap.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IUserService _userService;
        private readonly IFileStorageService _fileStorage;
        public CartController(ICartService cartService, IUserService userService, IFileStorageService fileStorage)
        {
            _cartService = cartService;
            _userService = userService;
            _fileStorage = fileStorage;
        }
        public IActionResult Index()
        {
            CartDetailsViewModel cartDetails;
            if (User.Identity.IsAuthenticated)
                cartDetails = _cartService.ShowCart(User.Identity.Name, null);
            else
                cartDetails = _cartService.ShowCart(null, Request.Cookies["cart-token"]);

            return View(cartDetails);
        }
        public IActionResult DownloadCartFile(int cartItemFileId)
        {
            var file = _cartService.GetCartItemFile(cartItemFileId);
            if (file == null) return NotFound();

            var cart = file.CartItem.Cart;

            bool hasAccess = false;
            if (User.Identity?.IsAuthenticated == true)
            {
                int userId = _userService.GetUserIdWithUserName(User.Identity.Name);
                hasAccess = cart.UserID == userId;
            }
            else
            {
                var token = Request.Cookies["cart-token"];
                hasAccess = !string.IsNullOrEmpty(token) && token == cart.CartToken;
            }

            if (!hasAccess) return Forbid();

            var path = _fileStorage.GetTempFilePath(file.CartItem.CartItemID, file.FileName);

            if (!System.IO.File.Exists(path)) return NotFound();

            return PhysicalFile(path, "application/octet-stream", file.OriginalFileName);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteCartItem(int cartItemId)
        {
            var result = _cartService.DeleteCartItem(cartItemId);
            if (!result)
                return Json(new { success = false, message = "آیتم مورد نظر پیدا نشد." });

            string userName = User.Identity.IsAuthenticated ? User.Identity.Name : null;
            string token = Request.Cookies["cart-token"];

            var summary = _cartService.GetCartSummary(userName, token);

            return Json(new
            {
                success = true,
                totalItems = summary.TotalItems,
                totalPrice = summary.TotalPrice
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult SelectAddress(int selectedAddressId)
        {
            var cart = _cartService.GetUserActiveCart(User.Identity.Name);
            if (cart == null || !cart.CartItems.Any())
                return RedirectToAction("Index");

            bool addressSet = _cartService.SetSelectedAddress(cart.CartID, selectedAddressId, User.Identity.Name);
            if (!addressSet)
            {
                TempData["CheckoutError"] = "آدرس انتخابی معتبر نیست.";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", "Checkout");
        }
    }
}
