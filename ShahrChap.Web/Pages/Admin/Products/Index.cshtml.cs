using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.DTOs.Products;
using ShahrChap.Core.Services.Interfaces;

namespace ShahrChap.Web.Pages.Admin.Products
{
    public class IndexModel : PageModel
    {
        private IProductService _productService;
        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }
        public List<ShowProductForAdminViewModel> ProductList { get; set; }
        public void OnGet()
        {
            ProductList = _productService.GetProductsForAdmin();
        }
    }
}
