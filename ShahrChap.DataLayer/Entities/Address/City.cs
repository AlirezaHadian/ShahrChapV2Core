using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShahrChap.DataLayer.Entities.Address;

public class City
{
    public City()
    {
        
    }
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int CityId { get; set; }

    [Display(Name = "شهرستان")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(250, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
    public string CityName { get; set; }

    [Display(Name = "استان")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int ProvinceId { get; set; }
    
    #region Relations
    public virtual Province Province { get; set; }
    public virtual List<UserAddress> UserAddresses { get; set; }
    #endregion
}