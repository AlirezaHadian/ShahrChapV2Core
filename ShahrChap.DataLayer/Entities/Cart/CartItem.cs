using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShahrChap.DataLayer.Entities.Cart
{
    public class CartItem
    {
        [Key]
        public int CartItemID { get; set; }
        public int CartID { get; set; }
        public int ProductID { get; set; }
        public string ProductTitle { get; set; }
        public string FeaturesCombination { get; set; }
        public long CalculatedPrice { get; set; }

        #region Relations 
        public virtual List<CartItemService>? CartItemServices { get; set; } = new ();
        #endregion
    }
}
