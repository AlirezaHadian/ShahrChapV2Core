using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.DTOs
{
    public class ShowAddressViewModel
    {
        public int AddressId { get; set; }
        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(150, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        public string FullName { get; set; }
        [Display(Name = "آدرس کامل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage ="{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        public string FullAddress { get; set; }
        [Display(Name = "استان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Province { get; set; }
        [Display(Name = "شهرستان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string City { get; set; }
        [Display(Name = "کد پستی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string PostCode { get; set; }
        [Display(Name = "پلاک")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int HouseNumber { get; set; }
    }

    public class AddressViewModel {
        [Required]
        public int UserAddressId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(150, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        public string FullName { get; set; }
        [Display(Name = "آدرس کامل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        public string FullAddress { get; set; }
        [Display(Name = "استان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int ProvinceId { get; set; }
        [Display(Name = "شهرستان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int CityId { get; set; }
        [Display(Name = "کد پستی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string PostCode { get; set; }
        [Display(Name = "پلاک")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int HouseNumber { get; set; }
    }

}
