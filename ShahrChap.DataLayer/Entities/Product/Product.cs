using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ShahrChap.DataLayer.Entities.Product;

public class Product
{
    public Product()
    {
    }

    [Key]
    public int ProductId { get; set; }
    [Required]
    public int GroupId { get; set; }
    public int? SubGroupId { get; set; }
    [Display(Name = "نوع محصول")] 
    public int? ParentId { get; set; }

    [Display(Name = "عنوان نوع محصول")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(450, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
    public string ProductTitle { get; set; }

    [Display(Name = "توضیحات")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public string Description { get; set; }

    [Display(Name = "تصویر")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(50, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
    public string Image { get; set; } = "NoImage.jpg";

    [Display(Name = "نکات چاپ")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public string PrintingTips { get; set; }
    [Display(Name = "تگ ها")]
    [MaxLength(600)]
    public string Tags { get; set; }
    [Display(Name = "تاریخ ثبت")]
    public DateTime CreateDate { get; set; }
    [Display(Name = "حذف شده؟")]
    public bool IsDelete { get; set; }
    [Display(Name = "نیاز به طراحی؟")]
    public bool IsDesignable { get; set; }

    #region Relations
    [ForeignKey("GroupId")]
    public ProductGroup? Group { get; set; }
    [ForeignKey("SubGroupId")]
    public ProductGroup? SubGroup { get; set; }
    [ForeignKey("ParentId")]
    public List<Product>? Products { get; set; }
    public List<ProductGallery>? ProductGalleries { get; set; }
    public List<ProductPrice>? ProductPrices { get; set; }
    public List<Service>? Services { get; set; }
    public List<ProductFeature>? ProductFeatures { get; set; }
    public List<ProductFeatureValue>? ProductFeatureValues { get; set; }
    public DesignPrice? DesignPrice { get; set; }
    #endregion
}