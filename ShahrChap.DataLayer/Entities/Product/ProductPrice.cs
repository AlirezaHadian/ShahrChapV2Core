using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShahrChap.DataLayer.Entities.Product;

public class ProductPrice
{
    public ProductPrice()
    {

    }
    [Key]
    public int ProductPriceId { get; set; }
    [ForeignKey("Product")]
    public int ProductId { get; set; }
    [Display(Name = "ترکیب ویژگی ها")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public string Combination { get; set; }
    [Display(Name = "قیمت")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int Price { get; set; }
    
    #region Relations

    public virtual Product? Product { get; set; }
    public virtual List<ServicePrice>? ServicePrices { get; set; }

    #endregion
}