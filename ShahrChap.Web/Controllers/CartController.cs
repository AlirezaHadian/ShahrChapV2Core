using Microsoft.AspNetCore.Mvc;
using NuGet.DependencyResolver;
using ShahrChap.Core.DTOs.Cart;
using ShahrChap.Core.Services.Interfaces;
using System.Security.Claims;

namespace ShahrChap.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ICartService _cartService;
        private readonly IUserService _userService;
        private readonly IFileStorageService _fileStorage;
        public CartController(IWebHostEnvironment env, ICartService cartService, IUserService userService, IFileStorageService fileStorage)
        {
            _env = env;
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
        public async Task<IActionResult> DeleteCartItem(int cartItemId, int orderDetailId)
        {

            var result = _cartService.DeleteCartItem(cartItemId);

            return Json(new
            {
                success = result
            });
        }
    }
}
