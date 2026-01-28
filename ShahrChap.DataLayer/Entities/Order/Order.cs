using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShahrChap.DataLayer.Entities.Order
{
    public class Order
    {
        public Order()
        {

        }

        [Key]
        public int OrderId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int OrderStatusId { get; set; }
        [Display(Name = "قیمت کل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long TotalPrice { get; set; }
        [Display(Name = "تخفیف")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long DiscountAmount { get; set; }
        [Display(Name = "قیمت قابل پرداخت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long FinalPrice { get; set; }
        [Display(Name = "تاریخ ثبت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public DateTime CreateDate { get; set; }

        #region Relations
        public virtual User.User User { get; set; }
        public virtual OrderStatus Status { get; set; }
        public virtual List<OrderDetail> OrderStatus { get; set; }
        #endregion
    }
}
