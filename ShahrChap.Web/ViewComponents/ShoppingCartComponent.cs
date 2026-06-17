using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using ShahrChap.Core.DTOs.Cart;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Cart;

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
