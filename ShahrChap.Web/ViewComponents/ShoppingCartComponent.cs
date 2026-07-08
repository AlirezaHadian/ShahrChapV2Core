using Microsoft.AspNetCore.Mvc;
using ShahrChap.Core.DTOs.Cart;
using ShahrChap.Core.Services.Interfaces;

namespace ShahrChap.Web.ViewComponents
{
    public class ShoppingCartComponent : ViewComponent
    {
        private ICartService _cartService;
        public ShoppingCartComponent(ICartService cartService)
        {
            _cartService = cartService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            CartForPopoverViewModel cart = new();
            string token = Request.Cookies["cart-token"];

            if (User.Identity.IsAuthenticated)
                cart = _cartService.ShowCartForPopover(userName: User.Identity.Name);
            else if (token != null)
                cart = _cartService.ShowCartForPopover(token: token);

            return await Task.FromResult((IViewComponentResult) View("ShoppingCart", cart));
        }
    }
}
