using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShahrChap.DataLayer.Entities.Product;

public class ProductFeatureValue
{
    [Key]
    public int PF_ValueId { get; set; }
    public int ProductId { get; set; }
    public int FeatureId { get; set; }
    public int FeatureValueId { get; set; }

    #region Relations

    public Product Product { get; set; } = null!;
    public Feature Feature { get; set; } = null!;
    public FeatureValue FeatureValue { get; set; } = null!;
    #endregion
}