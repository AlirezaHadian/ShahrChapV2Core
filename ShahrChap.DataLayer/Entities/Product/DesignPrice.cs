using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.DataLayer.Entities.Product
{
    public class DesignPrice
    {
        public DesignPrice()
        {
            
        }
        [Key]
        public int DesignPriceId { get; set; }
        public int ProductPriceId { get; set; }
        [Display(Name = "قیمت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Price { get; set; }

        #region Relations
        [ForeignKey("ProductPriceId")]
        public ProductPrice? ProductPrice { get; set; }
        #endregion
    }
}
