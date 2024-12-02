using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShahrChap.DataLayer.Entities.Product
{
    public class Service
    {
        public Service()
        {

        }
        [Key]
        public int ServiceId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [Display(Name = "عنوان خدمات پس از فروش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(150, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
        public string Title { get; set; }
        [Display(Name = "حذف شده؟")]
        public bool IsDelete { get; set; }

        #region Relations
        public virtual Product Product { get; set; }
        public virtual List<ServicePrice> ServicePrices { get; set; }
        #endregion
    }
}