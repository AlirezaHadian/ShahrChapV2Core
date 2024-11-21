using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ShahrChap.DataLayer.Entities.Product.Service;

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
    public string Image { get; set; }
    [Display(Name = "نکات چاپ")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public string PrintingTips { get; set; }
    [Display(Name = "تاریخ ثبت")]
    public DateTime CreateDate { get; set; }
    [Display(Name = "حذف شده؟")]
    public bool IsDelete { get; set; }

    #region Relations
    public virtual ProductGroup ProductGroup { get; set; }
    [ForeignKey("ParentId")]
    public virtual List<Product> Products { get; set; } 
    public virtual List<ProductGallery> ProductGalleries { get; set; }
    public virtual List<ProductPrice> ProductPrices { get; set; }
    public virtual List<ProductService> ProductServices { get; set; }
    public virtual List<ProductFeature> ProductFeatures { get; set; }
    public virtual List<Tag> Tags { get; set; }
    #endregion
}