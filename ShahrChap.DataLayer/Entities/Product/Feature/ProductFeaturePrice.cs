using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShahrChap.DataLayer.Entities.Product;

public class ProductFeaturePrice
{
    public ProductFeaturePrice()
    {

    }

    [Key]
    public int ProductFeaturePriceId { get; set; }
    [ForeignKey("Product")]
    public int ProductId { get; set; }
    [ForeignKey("ProductFeature")]
    public int ProductFeatureId { get; set; }
    [ForeignKey("ProductFeatureValue")]
    public int ProductFeatureValueId { get; set; }
    [Display(Name = "قیمت")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int Price { get; set; }
    
    #region Relations
    public virtual Product Product { get; set; }
    public virtual ProductFeature ProductFeature { get; set; }
    public virtual ProductFeatureValue ProductFeatureValue { get; set; }

    #endregion
}