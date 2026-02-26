using ShahrChap.DataLayer.Entities.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShahrChap.DataLayer.Entities.Cart
{
    public class CartItemService
    {
        [Key]
        public int CartItemServiceID { get; set; }
        public int CartItemID { get; set; }
        public int ServiceID { get; set; }

        #region Relations 
        public virtual CartItem CartItem { get; set; }
        public virtual Service Service { get; set; }
        #endregion
    }
}
