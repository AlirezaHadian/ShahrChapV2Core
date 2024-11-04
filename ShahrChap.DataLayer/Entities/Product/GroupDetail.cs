using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.DataLayer.Entities.Product
{
    public class GroupDetail
    {
        [Key]
        public int DG_Id { get; set; }
        public int ProductGroupId { get; set; }
        [Display(Name = "تصویر گرووه محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        public string Image { get; set; }
        [Display(Name = "توضیحات محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; }

        #region Relations
        public virtual ProductGroup ProductGroup { get; set; }
        #endregion
    }
}
