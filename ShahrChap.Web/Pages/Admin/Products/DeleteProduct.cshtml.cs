using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Convertors;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Pages.Admin.Products
{
    public class DeleteProductModel : PageModel
    {
        private IProductService _productService;
        public DeleteProductModel(IProductService productService)
        {
            _productService = productService;
        }
        [BindProperty]
        public Product Product { get; set; }
        public void OnGet(int id)
        {
            Product = _productService.GetProductById(id);
        }

        public IActionResult OnPost()
        {
            Product = _productService.GetProductById(Product.ProductId);
            _productService.DeleteProduct(Product);
            return RedirectToPage("Index");
        }
    }
}
