using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShahrChap.DataLayer.Entities.Product
{
    public class ServicePrice
    {
        public ServicePrice()
        {

        }
        [Key]
        public int ServicePriceId { get; set; }
        [ForeignKey("ProductServiceId")]
        public int ProductServiceId { get; set; }
        [ForeignKey("ProductPrice")]
        public int ProductPriceId { get; set; }
        [Display(Name = "قیمت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Price { get; set; }
        #region Relations
        public virtual Service Service { get; set; }
        public virtual ProductPrice ProductPrice { get; set; }
        #endregion
    }
}