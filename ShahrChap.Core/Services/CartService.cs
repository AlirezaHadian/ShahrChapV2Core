using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Context;
using ShahrChap.DataLayer.Entities.Cart;
using ShahrChap.DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.Services
{
    public class CartService : ICartService
    {
        private ShahrChapContext _context;
        private IUserService _userService;
        public CartService(ShahrChapContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }
        #region Cart
        public Cart GetUserCart(string userName)
        {
            int userId = _userService.GetUserIdWithUserName(userName);
            Cart cart = _context.Carts.FirstOrDefault(c => c.UserID == userId);
            return cart;
        }
        #endregion
    }
}
