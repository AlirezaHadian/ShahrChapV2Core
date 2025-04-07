using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.DataLayer.Entities.Product.Form
{
    public class ProductForm
    {
        public ProductForm() { }
        [Key]
        public int ProductFormId { get; set; }
        public int ProductId { get; set; }

        [Display(Name = "عنوان فرم")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        public string ProductFormTitle { get; set; }
        [Display(Name = "توضیحات")]
        public string? Description { get; set; }
        public bool IsDelete { get; set; }
        [Display(Name = "فرم طراحی؟")]
        public bool IsDesignable { get; set; }

        #region Relations
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public List<FormInput> FormInputs { get; set; }
        #endregion
    }
}
