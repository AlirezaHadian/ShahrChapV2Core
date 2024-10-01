using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShahrChap.DataLayer.Entities.Address;

public class Province
{
    public Province()
    {
        
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)] // No auto-increment
    public int ProvinceId { get; set; }

    [Display(Name = "استان")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(250, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
    public string ProvinceName { get; set; }

    #region Relations
    public virtual List<City> Cities { get; set; }
    public virtual List<UserAddress> UserAddresses { get; set; }
    #endregion
}