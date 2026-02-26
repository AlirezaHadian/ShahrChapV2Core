using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ShahrChap.DataLayer.Entities.Cart
{
    public class Cart
    {
        [Key]
        public int CartID { get; set; }
        [AllowNull]
        public int? UserID { get; set; }
        [AllowNull]
        public string? CartToken { get; set; }
        public DateTime CreateDate { get; set; }

        #region Relations
        public virtual User.User User { get; set; }
        public virtual List<CartItem> CartItems { get; set; }
        #endregion
    }
}
