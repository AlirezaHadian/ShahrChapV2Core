using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShahrChap.DataLayer.Entities.Product.Service
{
    public class ProductServicePrice
    {
        public ProductServicePrice()
        {
            
        }
        [Key]
        public int ProductServicePriceId { get; set; }
        [ForeignKey("ProductService")]
        public int ProductServiceId { get; set; }
        [ForeignKey("ProductPrice")]
        public int ProductPriceId { get; set; }
        [Display(Name = "قیمت")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Price { get; set; }
        [Display(Name = "حذف شده؟")]
        public bool IsDelete { get; set; }
        #region Relations
        public virtual ProductService ProductService { get; set; }
        public virtual ProductPrice ProductPrice { get; set; }
        #endregion
    }
}