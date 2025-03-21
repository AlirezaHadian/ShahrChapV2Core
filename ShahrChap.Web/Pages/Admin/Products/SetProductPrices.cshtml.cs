using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Pages.Admin.Products
{
    public class SetProductPricesModel : PageModel
    {
        private IProductService _productService;
        public SetProductPricesModel(IProductService productService)
        {
            _productService = productService;
        }

        public Product Product { get; set; }
        //public List<string> FeatureCombinations { get; set; }
        public List<Service> ProductServices { get; set; }
        [BindProperty]
        public List<ProductPrice> Prices { get; set; }
        [BindProperty]
        public List<ServicePrice> ServicePrices { get; set; }
        public void OnGet(int id)
        {
            Product = _productService.GetProductById(id);

            ProductServices = _productService.GetProductServices(Product.ParentId.Value);

            Prices = _productService.GetProductPrices(id);
            
            foreach (var price in Prices)
            {
                if (price.DesignPrice == null)
                {
                    price.DesignPrice = new DesignPrice { Price = 0 };
                }
            }

            ServicePrices = _productService.GetServicePricesForProduct(id);
        }

        public IActionResult OnPost(int productId)
        {
            Product = _productService.GetProductById(productId);

            ProductServices = _productService.GetProductServices(Product.ParentId.Value);

            if (!ModelState.IsValid) return Page();

            _productService.UpdateProductPrices(Product.ProductId, Prices);

            return RedirectToPage("IndexSubProduct", new { id= Product.ParentId });
        }

        
    }
}
