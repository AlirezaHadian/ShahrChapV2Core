using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace ShahrChap.DataLayer.Entities.Product;

public class ProductGallery
{
    public ProductGallery()
    {
        
    }
[Key]
    public int ProductGalleryId { get; set; }
    [Display(Name = "محصول")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int ProductId { get; set; }
    [Display(Name = "عنوان تصویر")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(50, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
    public string ImageTitle { get; set; }
    [Display(Name = "تصویر")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(50, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
    public string ImageName { get; set; }
    [Display(Name = "حذف شده؟")]
    public bool IsDelete { get; set; }
    #region Relations
    public virtual Product? Product { get; set; }
    #endregion
}