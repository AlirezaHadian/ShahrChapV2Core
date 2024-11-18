using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShahrChap.DataLayer.Entities.Product
{
    public class Tag
    {
        public Tag()
        {

        }
        [Key]
        public int TagId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [Display(Name = "تگ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "{0} نمی تواند بیش از {1} کاراکتر باشد")]
        public string TagTitle { get; set; }
        [Display(Name = "حذف شده؟")]
        public bool IsDelete { get; set; }

        #region Relations
        public virtual Product Product { get; set; }
        #endregion
    }
}