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
    [Display(Name = "مقدار ویژگی")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(350, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
    public string ValueTitle { get; set; }
    [Display(Name = "حذف شده؟")]
    public bool IsDelete { get; set; }

    #region Relations
    public virtual ProductFeature ProductFeature { get; set; }
    #endregion
}