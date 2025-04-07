using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;
using ShahrChap.DataLayer.Entities.Product.Form;

namespace ShahrChap.Web.Pages.Admin.Form
{
    public class EditFormModel : PageModel
    {
        private IProductService _productService;
        public EditFormModel(IProductService productService)
        {
            _productService = productService;
        }
        public Product Product { get; set; }
        [BindProperty]
        public ProductForm Form { get; set; }
        public void OnGet(int id)
        {
            Form = _productService.GetProductFormById(id);
        }
    }
}
