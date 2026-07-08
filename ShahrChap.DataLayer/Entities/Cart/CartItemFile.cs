using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShahrChap.DataLayer.Entities.Cart
{
    public class CartItemFile
    {
        [Key]
        public int CartItemFileId { get; set; }
        public int CartItemID { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public DateTime UploadDate { get; set; }

        public virtual CartItem CartItem { get; set; }
    }
}
