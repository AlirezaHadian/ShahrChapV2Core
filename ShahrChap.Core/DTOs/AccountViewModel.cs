using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.DTOs
{
    public class RegisterViewModel
    {
        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "{0} اجباری می باشد")]
        [MaxLength(250, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        public string UserName { get; set; }

        [Display(Name = "ایمیل / شماره موبایل")]
        [Required(ErrorMessage = "{0} اجباری می باشد")]
        [MaxLength(450, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        [RegularExpression(@"(00989|\+989|989|09)(\d{9})|([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})", ErrorMessage = "{0} معتبر نمی باشد")]
        public string EmailOrPhone { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "{0} اجباری می باشد")]
        [MaxLength(50, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "تکرار کلمه عبور")]
        [Required(ErrorMessage = "{0} اجباری می باشد")]
        [MaxLength(50, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        [Compare("Password", ErrorMessage = "کلمه های عبور مطابقت ندارند")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
    public class LoginViewModel
    {
        [Display(Name = "ایمیل / شماره موبایل")]
        [Required(ErrorMessage = "{0} اجباری می باشد")]
        [MaxLength(450, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        [RegularExpression(@"(00989|\+989|989|09)(\d{9})|([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})", ErrorMessage = "{0} معتبر نمی باشد")]
        public string EmailOrPhone { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "{0} اجباری می باشد")]
        [MaxLength(50, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "مرا بخاطر بسپار")]
        public bool RememberMe { get; set; }
    }
    public class VerifyPhoneViewModel
    {
        [Display(Name = "شماره موبایل")]
        public string PhoneNumber { get; set; }
        [Display(Name = "کد فعالسازی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Otp { get; set; }
        public string ActionType { get; set; }
    }
    public class ForgotPasswordViewModel
    {
        [Display(Name = "ایمیل / شماره موبایل")]
        [Required(ErrorMessage = "{0} اجباری می باشد")]
        [MaxLength(450, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        [RegularExpression(@"(00989|\+989|989|09)(\d{9})|([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})", ErrorMessage = "{0} معتبر نمی باشد")]
        public string EmailOrPhone { get; set; }
    }
    public class ResetPasswordEmailViewModel
    {
        public string ResetValue { get; set; }
        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "{0} اجباری می باشد")]
        //[MinLength(8, ErrorMessage = "{0} باید بیشتر از ۸ کاراکتر باشد")]
        public string Password { get; set; }
        [Display(Name = "تکرار کلمه عبور")]
        [Required(ErrorMessage = "{0} اجباری می باشد")]
        //[MinLength(8, ErrorMessage = "{0} باید بیشتر از ۸ کاراکتر باشد")]
        public string ConfirmPassword { get; set; }
    }

}
