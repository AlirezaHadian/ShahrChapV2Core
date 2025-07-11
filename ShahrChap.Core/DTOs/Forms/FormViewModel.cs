using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.DTOs.Forms
{
    public class FormInputModel
    {
        public int? Id { get; set; }

        [Display(Name = "عنوان ورودی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "{0} نمی‌تواند بیش از {1} کاراکتر باشد")]
        public string InputName { get; set; }

        [Display(Name = "نوع ورودی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی‌تواند بیش از {1} کاراکتر باشد")]
        public string InputType { get; set; }

        [Display(Name = "مقادیر")]
        [MaxLength(450, ErrorMessage = "{0} نمی‌تواند بیش از {1} کاراکتر باشد")]
        public string? Options { get; set; }

        [Display(Name = "آیا این فیلد اجباری است؟")]
        public bool IsRequired { get; set; }

        // برای اتصال به مقادیر ویژگی
        [Display(Name = "مقدار ویژگی")]
        public int? ProductFeatureValueId { get; set; }

        // برای اتصال به خدمات پس از چاپ
        [Display(Name = "خدمات پس از چاپ")]
        public int? ServiceId { get; set; }

        // برای نمایش اطلاعات مرتبط در View
        public string? FeatureValueTitle { get; set; }
        public string? ServiceTitle { get; set; }
    }
}
