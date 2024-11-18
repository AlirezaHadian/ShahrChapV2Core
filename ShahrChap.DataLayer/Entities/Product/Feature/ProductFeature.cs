using System.ComponentModel.DataAnnotations;

namespace ShahrChap.DataLayer.Entities.Product;

public class ProductFeature
{
    public ProductFeature()
    {
        
    }
    [Key]
    public int ProductFeatureId { get; set; }
    [Display(Name = "عنوان ویژگی")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(200, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
    public string FeatureTitle { get; set; }
    [Display(Name = "حذف شده؟")]
    public bool IsDelete { get; set; }

    #region Relations

    public virtual List<ProductFeatureValue> ProductFeatureValues { get; set; }
    #endregion
}