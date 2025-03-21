using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.DataLayer.Entities.Product.Form
{
    public class FormInput
    {
        public FormInput()
        {
            
        }
        [Key]
        public int FormInputId { get; set; }
        public int ProductFormId { get; set; }
        public int? FeatureValueId { get; set; }
        public int? ServiceId { get; set; }
        [Display(Name = "عنوان ورودی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
        public string InputName { get; set; }
        [Display(Name = "نوع ورودی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
        public string InputType { get; set; }
        [Display(Name = "مقادیر")]
        [MaxLength(450, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
        public string? Options { get; set; }


        #region Relations
        public ProductForm? ProductForm { get; set; }
        [ForeignKey("FeatureValueId")]
        public FeatureValue? FeatureValue { get; set; }
        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }
        #endregion
    }
}
