using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.DataLayer.Entities.Product
{
    public class ProductGroup
    {
        public ProductGroup()
        {

        }
        [Key]
        public int GroupId { get; set; }
        [Display(Name = "عنوان گروه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(250, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
        public string GroupTitle { get; set; }
        [Display(Name = "آیکون")]
        public string? IconClass { get; set; }
        [Display(Name = "حذف شده؟")]
        public bool IsDelete { get; set; }
        [Display(Name = "گروه اصلی")]
        public int? ParentId { get; set; }

        #region Relation
        [ForeignKey("ParentId")]
        public virtual List<ProductGroup> ProductGroups { get; set; }
        public virtual List<Product> Products { get; set; }
        #endregion
    }
}
