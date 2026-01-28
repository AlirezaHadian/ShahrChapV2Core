using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShahrChap.DataLayer.Entities.Order
{
    public class OrderStatus
    {
        public OrderStatus()
        {

        }
        [Key]
        public int StatusId { get; set; }
        [Display(Name = "عنوان وضعیت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
        public string StatusTitle { get; set; }
        [Display(Name = "ترتیب")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int SortOrder { get; set; }
        public string OrderColor { get; set; }
        public bool IsDelete { get; set; }

        #region Relations
        public virtual List<Order>? Orders { get; set; }
        #endregion
    }
}
