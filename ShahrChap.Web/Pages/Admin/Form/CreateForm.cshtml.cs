using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Enums;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;
using ShahrChap.DataLayer.Entities.Product.Form;

namespace ShahrChap.Web.Pages.Admin.Form
{
    public class CreateFormModel : PageModel
    {
        private IProductService _productService;

        public CreateFormModel(IProductService productService)
        {
            _productService = productService;
        }
        public Product Product { get; set; }
        [BindProperty]
        public ProductForm Form { get; set; }
        public List<ProductForm> ProductForms { get; set; }
        public FormCreationState State { get; set; }
        public IActionResult OnGet(int id)
        {
            Product = _productService.GetProductById(id);
            State = _productService.GetPendingForms(id);

            if (State != FormCreationState.NoFormAllowed)
            {
                ProductForms = _productService.GetProductForms(id);

                return Page();
            }
            return NotFound();
        }

        public IActionResult OnPost()
        {
            Product = _productService.GetProductById(Form.ProductId);
            ProductForms = _productService.GetProductForms(Form.ProductId);

            if (!ModelState.IsValid)
                return Page();

            return Page();
        }
    }
}
