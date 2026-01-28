using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShahrChap.DataLayer.Entities.Order
{
    public class OrderFile
    {
        public OrderFile()
        {
            
        }
        [Key]
        public int FileId { get; set; }
        [Required]
        public int DetailId { get; set; }
        [Required]
        public string FileName { get; set; }

        #region Relations
        public virtual OrderDetail Detail { get; set; }
        #endregion
    }
}
