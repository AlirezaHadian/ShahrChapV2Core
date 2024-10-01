using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShahrChap.DataLayer.Entities.Address;

namespace ShahrChap.DataLayer.Entities.User
{
    public class User
    {
        public User()
        {
            
        }

        [Key]
        public int UserId { get; set; }

        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(250, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        public string UserName { get; set; }

        [Display(Name = "ایمیل")]
        [MaxLength(450, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد")]
        public string? Email { get; set; }

        [Display(Name = "شماره موبایل")]
        [MaxLength(150, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        [Phone(ErrorMessage = "شماره موبایل وارد شده معتبر نمی باشد")]
        [RegularExpression(@"09([0-9][0-9]|3[1-9]|2[1-9])-?[0-9]{3}-?[0-9]{4}$", ErrorMessage = "شماره موبایل وارد شده معتبر نمی باشد")]
        public string? Phone { get; set; }

        [Display(Name = "کلمه عبور")]
        [MaxLength(50, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        public string Password { get; set; }

        [Display(Name = "کد فعالسازی")]
        [MaxLength(50, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        public string ActiveCode { get; set; }

        [Display(Name = "آواتار")]
        [MaxLength(150, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        public string UserAvatar { get; set; }

        [Display(Name = "تاریخ ثبت نام")]
        public DateTime RegisterDate { get; set; }

        [Display(Name = "وضعیت ایمیل")]
        public bool IsEmailActive { get; set; }

        [Display(Name = "وضعیت شماره موبایل")]
        public bool IsPhoneActive { get; set; }


        #region Relation
        public virtual List<UserRole> UserRoles { get; set; }
        public virtual List<Wallet.Wallet> Wallets { get; set; }
        public virtual List<UserAddress> UserAddresses { get; set; }
        #endregion
    }
}

