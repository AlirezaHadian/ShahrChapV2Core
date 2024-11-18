using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShahrChap.DataLayer.Entities.Product;

public class ProductFeatureValue
{
    public ProductFeatureValue()
    {
        
    }
[Key]
    public int ProductFeatureValueId { get; set; }
    [Required]
    public int ProductFeatureId { get; set; }
    
    public string ValueTitle { get; set; }
    public bool IsDelete { get; set; }

    #region Relations

    public virtual ProductFeature ProductFeature { get; set; }
    
    #endregion
}