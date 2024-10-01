using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShahrChap.DataLayer.Entities.Address;

public class UserAddress
{
    public UserAddress()
    {
        
    }
    
    [Key]
    public int UserAddressId { get; set; }

    [Display(Name = "کاربر")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int UserId { get; set; }
    [Display(Name = "استان")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int ProvinceId { get; set; }
    [Display(Name = "شهرستان")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int CityId { get; set; }
    [Display(Name = "نام و نام خانوادگی")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(150, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
    public string FullName { get; set; }
    [Display(Name = "آدرس خانه")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(300, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
    public string FullAddress { get; set; }
    [Display(Name = "کد پستی")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [DataType(DataType.PostalCode)]
    [RegularExpression(@"\b(?!(\d)\1{3})[13-9]{4}[1346-9][013-9]{5}\b", ErrorMessage = "کد پستی وارد شده نامعتبر می باشد.")]
    public string PostCode { get; set; }
    [Display(Name = "پلاک")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int HouseNumber { get; set; }

    #region Relations
    public virtual User.User User { get; set; }
    public virtual Province Province { get; set; }
    public virtual City City { get; set; }
    #endregion
}