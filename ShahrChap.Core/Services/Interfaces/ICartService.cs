using ShahrChap.Core.DTOs.Cart;
using ShahrChap.DataLayer.Entities.Cart;
using ShahrChap.DataLayer.Entities.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.Services.Interfaces
{
    public interface ICartService
    {
        Cart GetUserActiveCart(string userName = null, string token = null);
        Cart CreateCartOrAddItem(int productId, string featuresCombination, List<int> serviceIds, string userName = null, string token=null, Cart cart = null);
        CartForPopoverViewModel ShowCartForPopover(string userName = null, string token = null);
        public List<CartItem> GetUserCartItems(int cartId);
        CartDetailsViewModel ShowCart(string userName = null, string token = null);
        OrderFile GetOrderFileWithFileName(int fileId);
        bool DeleteCartItem(int cartItemId, int orderDetailId);
    }
}
