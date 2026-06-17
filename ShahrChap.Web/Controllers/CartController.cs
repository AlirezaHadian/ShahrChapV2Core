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
        public CartController(IWebHostEnvironment env, ICartService cartService, IUserService userService)
        {
            _env = env;
            _cartService = cartService;
            _userService = userService;
        }
        public IActionResult Index()
        {
            CartDetailsViewModel cartDetails;
            if (User.Identity.IsAuthenticated)
                cartDetails = _cartService.ShowCart(User.Identity.Name);
            else
                cartDetails = _cartService.ShowCart(Request.Cookies["cart-token"]);

            return View(cartDetails);
        }

        public IActionResult DownloadFile(int fileId)
        {
            var file = _cartService.GetOrderFileWithFileName(fileId);

            if (file == null)
                return NotFound();

            var order = file.Detail.Order;

            bool hasAccess = false;
            if (User.Identity?.IsAuthenticated == true)
            {
                int userId = _userService.GetUserIdWithUserName(User.Identity.Name);

                hasAccess = order.UserId == userId;
            }
            else
            {
                var token = Request.Cookies["cart-token"];

                hasAccess = !string.IsNullOrEmpty(token)
                    && token == order.CheckoutToken;
            }

            if (!hasAccess)
                return Forbid();

            var path = Path.Combine(_env.WebRootPath, "Uploads", "Temp", "Temp_" + fileId, file.FileName);

            if (!System.IO.File.Exists(path))
                return NotFound();


            return PhysicalFile(
                path,
                "application/octet-stream",
                file.OriginalFileName);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCartItem(int cartItemId, int orderDetailId)
        {
            int userId = _userService.GetUserIdWithUserName(User.Identity.Name);

            var result = _cartService.DeleteCartItem(cartItemId, orderDetailId);

            return Json(new
            {
                success = result
            });
        }
    }
}
