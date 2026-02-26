using ShahrChap.DataLayer.Entities.Cart;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.Services.Interfaces
{
    public interface ICartService
    {
        Cart GetUserCart(string userName);
    }
}
