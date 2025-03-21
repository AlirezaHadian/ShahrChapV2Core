using ShahrChap.DataLayer.Entities.Product.Form;
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
    [Display(Name = "حذف شده؟")]
    public bool IsDelete { get; set; }

    #region Relations
    public Feature? Feature { get; set; }
    public List<ProductFeatureValue>? ProductFeatureValues { get; set; }
    public List<FormInput>? FormInputs { get; set; }
    #endregion
}