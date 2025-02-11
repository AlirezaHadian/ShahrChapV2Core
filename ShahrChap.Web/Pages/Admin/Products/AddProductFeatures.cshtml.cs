using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

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
        [BindProperty]
        public Feature Feature { get; set; }

        public void OnGet(int id)
        {
            CurrentProduct = _productService.GetProductById(id);
            Features = _productService.GetAllFeatures();
        }
        public IActionResult OnPost()
        {

            return Page();
        }
        public IActionResult OnPostAddOrEdit([FromBody] Feature feature)
        {
            if (!ModelState.IsValid)
                return new JsonResult(new { success = false, message = "اطلاعات نامعتبر است" });

            if (feature.FeatureId > 0) // Edit Feature
            {
                Feature existingFeature = _productService.GetFeatureById(feature.FeatureId);
                if (existingFeature != null)
                {
                    existingFeature.FeatureTitle = feature.FeatureTitle;
                    _productService.UpdateFeature(existingFeature);
                }
            }
            else // Add New Feature
            {
                _productService.AddFeature(feature);
            }

            return new JsonResult(new { success = true });
        }

        public IActionResult OnPostDelete([FromBody] int featureId)
        {
            Feature feature = _productService.GetFeatureById(featureId);
            if (feature == null)
            {
                return new JsonResult(new { success = false, message = "ویژگی یافت نشد" });
            }

            //_productService.DeleteFeature();
            return new JsonResult(new { success = true });
        }
    }
}
