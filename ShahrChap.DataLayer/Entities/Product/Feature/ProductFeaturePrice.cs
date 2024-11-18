using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShahrChap.DataLayer.Entities.Product;

public class ProductFeatureMapping
{
    public ProductFeatureMapping()
    {
        
    }
[Key]
    public int ProductFeatureMappingId { get; set; }
    [ForeignKey("Product")]
    public int ProductId { get; set; }
    [ForeignKey("ProductFeature")]
    public int ProductFeatureId { get; set; }
    [ForeignKey("ProductFeatureValue")]
    public int ProductFeatureValueId { get; set; }
    
    #region Relations

    public virtual Product Product { get; set; }
    public virtual ProductFeature ProductFeature { get; set; }
    public virtual ProductFeatureValue ProductFeatureValue { get; set; }

    #endregion
}