using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.DataLayer.Entities.Product
{
    public class ProductType
    {
        public ProductType()
        {

        }

        [Key]
        public int ProductTypeId { get; set; }
        [Display(Name ="عنوان نوع")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(150, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
        public string TypeTitle { get; set; }
        [Display(Name = "تعداد فرم")]
        public int FormsCount { get; set; }

        #region Relations
        public List<Product> Products { get; set; }
        #endregion
    }
}
