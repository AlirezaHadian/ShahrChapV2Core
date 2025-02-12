using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Pages.Admin.Products
{
    public class ProductFeaturesModel : PageModel
    {
        private IProductService _productService;
        public ProductFeaturesModel(IProductService productService)
        {
            _productService = productService;
        }
        public Product CurrentProduct { get; set; }
        public List<ProductFeature> ProductFeatures { get; set; }
        public void OnGet(int id)
        {
            ViewData["ProductId"] = id;
            CurrentProduct = _productService.GetProductById(id);
            ProductFeatures = _productService.GetProductFeatures(id);
        }
    }
}
