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
        public List<string> FeatureCombinations { get; set; }
        public List<Service> ProductServices { get; set; }
        [BindProperty]
        public List<ProductPrice> Prices { get; set; }
        public void OnGet(int id)
        {
            Product = _productService.GetProductById(id);

            // Get all feature combinations
            FeatureCombinations = _productService.GetFeatureCombinations(id);
            ProductServices = _productService.GetProductServices(Product.ParentId.Value);

            Prices = _productService.GetProductPrices(id);


            if (FeatureCombinations.Count != Prices.Count || _productService.AreCombinationsChanged(Product.ProductId, FeatureCombinations))
            {

                Prices = FeatureCombinations.Select(combination => new ProductPrice
                {
                    Combination = combination,
                    Price = 0
                }).ToList();
            }
        }

        public IActionResult OnPost(int productId, List<int> ServicePrices)
        {
            Product = _productService.GetProductById(productId);
            FeatureCombinations = _productService.GetFeatureCombinations(Product.ProductId);
            ProductServices = _productService.GetProductServices(Product.ParentId.Value);

            if (!ModelState.IsValid) return Page();

            _productService.UpdateProductPrices(Product.ProductId, Prices);

            // Create a dictionary to hold service prices
            //Dictionary<int, int> servicePrices = new Dictionary<int, int>();

            // Retrieve service prices from form and store them
            //for (int i = 0; i < ProductServices.Count; i++)
            //{
            //    string key = $"ServicePrices[{i}]";
            //    if (Request.Form.ContainsKey(key) && int.TryParse(Request.Form[key], out int price))
            //    {
            //        servicePrices[ProductServices[i].ServiceId] = price;
            //    }
            //}

            _productService.AddServicePrices(productId, ServicePrices, FeatureCombinations.Count);

            return RedirectToPage("IndexSubProduct", new { id= Product.ParentId });
        }

        
    }
}
