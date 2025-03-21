using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.DataLayer.Entities.Product.Form
{
    public class ProductForm
    {
        public ProductForm() { }
        [Key]
        public int ProductFormId { get; set; }
        public int ProductId { get; set; }
        [Display(Name = "توضیحات")]
        public string? Description { get; set; }
        [Display(Name = "فرم طراحی شده؟")]
        public bool IsDesigned { get; set; }
        public bool IsDelete { get; set; }

        #region Relations
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public List<FormInput> FormInputs { get; set; }
        #endregion
    }
}
