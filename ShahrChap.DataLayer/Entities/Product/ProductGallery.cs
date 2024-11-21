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
    public int ProudctId { get; set; }
    [Display(Name = "تصویر")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(50, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
    public string ImageName { get; set; }

    #region Relations
    public virtual Product Product { get; set; }
    #endregion
}