using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Pages.Admin.Products
{
    public class AddSubProductFeaturesModel : PageModel
    {
        private IProductService _productService;
        public AddSubProductFeaturesModel(IProductService productService)
        {
            _productService = productService;
        }
        public Product CurrentProduct { get; set; }
        public List<ProductFeature> ProductFeatures { get; set; }
        public List<FeatureValue> Values { get; set; }

        //Update product price models
        public List<ProductPrice> ProductPrices { get; set; }
        public List<string> FeatureCombinations { get; set; }
        public void OnGet(int id)
        {
            CurrentProduct = _productService.GetProductById(id);
            ProductFeatures = _productService.GetProductFeatures(CurrentProduct.ParentId.Value);
            Values = _productService.GetAllFeatureValues(CurrentProduct.ParentId.Value);
            ViewData["SelectedFeatureValues"] = _productService.SubProductFeatureValueIds(id);
        }

        public IActionResult OnPost(int ProductId, List<int> SelectedFeatureValue)
        {
            CurrentProduct = _productService.GetProductById(ProductId);

            ProductFeatures = _productService.GetProductFeatures(CurrentProduct.ParentId.Value);
            List<int> existingFeatureValueIds = _productService.SubProductFeatureValueIds(ProductId);
            ViewData["SelectedFeatureValues"] = existingFeatureValueIds;

            Values = _productService.GetAllFeatureValues(CurrentProduct.ParentId.Value);

            if (!ModelState.IsValid)
                return Page();

            //Check if any changes have made to the features
            if (!existingFeatureValueIds.OrderBy(x => x).SequenceEqual(SelectedFeatureValue.OrderBy(x => x)))
            {
                //Add Features to product with product id
                _productService.UpdateFeatureValuesProduct(ProductId, SelectedFeatureValue);
                
                // Add|Update Combintaions in Product Price table
                FeatureCombinations = _productService.GetFeatureCombinations(ProductId);

                ProductPrices = FeatureCombinations.Select(combination => new ProductPrice
                {
                    Combination = combination,
                    Price = 0
                }).ToList();

                //Update the exsiting product prices 
                _productService.AddProductPrices(ProductId, ProductPrices);
            }

            return RedirectToPage("IndexSubProduct", new { id = CurrentProduct.ParentId });
        }
    }
}
