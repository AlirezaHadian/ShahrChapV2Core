using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using ShahrChap.Core.Services;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;
using System.Diagnostics;

namespace ShahrChap.Web.Pages.Admin.Products
{
    public class AddProductFeaturesModel : PageModel
    {
        private IProductService _productService;
        public AddProductFeaturesModel(IProductService productService)
        {
            _productService = productService;
        }
        public Product CurrentProduct { get; set; }
        public List<Feature> Features { get; set; }

        public void OnGet(int id)
        {
            //GET THE SELECTED FEATURES FOR THIS PRODUCT AND CHECKED IN THE VIEW
            CurrentProduct = _productService.GetProductById(id);
            Features = _productService.GetAllFeatures();
            ViewData["SelectedFeatures"] = _productService.ProductFeatureIds(id);
        }

        public IActionResult OnPost(int ProductId, List<int> SelectedFeature)
        {
            CurrentProduct = _productService.GetProductById(ProductId);
            Features = _productService.GetAllFeatures();
            List<int> existingFeatureIds = _productService.ProductFeatureIds(ProductId);
            ViewData["SelectedFeatures"] = existingFeatureIds;

            Features = _productService.GetAllFeatures();

            if (!ModelState.IsValid)
                return Page();

            if (!existingFeatureIds.OrderBy(x => x).SequenceEqual(SelectedFeature.OrderBy(x => x)))
            {
                //Add Features to product with product id
                _productService.UpdateFeaturesProduct(ProductId, SelectedFeature);
            }
            return RedirectToPage("Index");
        }
    }
}
