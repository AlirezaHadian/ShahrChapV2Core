using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.DTOs.Products;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Pages.Admin.Products
{
    public class IndexSubProductModel : PageModel
    {
        private IProductService _productService;
        public IndexSubProductModel(IProductService productService)
        {
            _productService = productService;
        }
        public List<ShowProductForAdminViewModel> SubProducts { get; set; }
        public Product Product { get; set; }
        public void OnGet(int id)
        {
            ViewData["ProductId"] = id;
            Product = _productService.GetProductById(id);
            SubProducts = _productService.GetSubProductForAdmin(id);
        }
    }
}
