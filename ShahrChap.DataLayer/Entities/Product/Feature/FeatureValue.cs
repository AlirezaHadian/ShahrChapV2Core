using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShahrChap.DataLayer.Entities.Product;

public class FeatureValue
{
    public FeatureValue()
    {

    }
    [Key]
    public int FeatureValueId { get; set; }
    [Required]
    public int FeatureId { get; set; }
    [Display(Name = "مقدار ویژگی")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(350, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
    public string ValueTitle { get; set; }
    
    #region Relations
    public Feature Feature { get; set; } = null!;
    public List<ProductFeatureValue> ProductFeatureValues { get; set; }
    #endregion
}