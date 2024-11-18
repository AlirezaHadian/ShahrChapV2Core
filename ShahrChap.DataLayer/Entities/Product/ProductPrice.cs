using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShahrChap.DataLayer.Entities.Product.Service;

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
    [Column(TypeName ="json")]
    [Display(Name = "ویژگی ها")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public string Configuration { get; set; }
    [Display(Name = "قیمت")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int Price { get; set; }
    
    #region Relations

    public virtual Product Product { get; set; }
    public virtual ProductServicePrice ProductServicePrice { get; set; }

    #endregion
}