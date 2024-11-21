using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShahrChap.DataLayer.Entities.Product;

public class ProductFeature
{
    public ProductFeature()
    {
            
    }
    
    [Key]
    public int PF_ID { get; set; }
    [ForeignKey("FeatureId")]
    public int FeatureId { get; set; }
    [ForeignKey("ProductId")]
    public int ProductId { get; set; }

    #region Relations
    public virtual Feature Feature { get; set; }
    public virtual Product Product { get; set; }
    #endregion
}
