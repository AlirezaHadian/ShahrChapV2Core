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
        public void OnGet(int id)
        {
            CurrentProduct = _productService.GetProductById(id);
            ProductFeatures = _productService.GetProductFeatures(CurrentProduct.ParentId.Value);
            Values = _productService.GetAllFeatureValues(CurrentProduct.ParentId.Value);
            ViewData["SelectedFeatureValues"] = _productService.SubProductFeatureValues(id);
        }

        public IActionResult OnPost(int ProductId, List<int> SelectedFeatureValue)
        {
            CurrentProduct = _productService.GetProductById(ProductId);

            ProductFeatures = _productService.GetProductFeatures(CurrentProduct.ParentId.Value);
            ViewData["SelectedFeatureValues"] = _productService.SubProductFeatureValues(ProductId);

            Values = _productService.GetAllFeatureValues(CurrentProduct.ParentId.Value);

            if (!ModelState.IsValid)
                return Page();

            //Add Features to product with product id
            _productService.UpdateFeatureValuesProduct(ProductId, SelectedFeatureValue);

            return RedirectToPage("IndexSubProduct", new {id = CurrentProduct.ParentId});
        }
    }
}
