using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShahrChap.DataLayer.Entities.Order
{
    public class OrderDetail
    {
        public OrderDetail()
        {

        }
        [Key]
        public int DetailId { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Display(Name = "عنوان محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
        public string OrderDetailTitle { get; set; }
        [Display(Name = "عنوان محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(450, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
        public string ProductTitle { get; set; }
        [Display(Name = "ویژگی ها")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(600, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
        public string FeaturesCombination { get; set; }
        [Display(Name = "خدمات پس از چاپ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(600, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
        public string Services { get; set; }
        [Display(Name = "قیمت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long Price { get; set; }

        #region Relations
        public virtual Order Order { get; set; }
        public virtual Product.Product Product { get; set; }
        public virtual List<OrderFile> Files { get; set; }
        #endregion
    }
}
