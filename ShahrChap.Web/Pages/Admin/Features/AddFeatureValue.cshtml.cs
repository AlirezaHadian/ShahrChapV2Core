using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Pages.Admin.Features
{
    public class AddFeatureValueModel : PageModel
    {
        private IProductService _productService;
        public AddFeatureValueModel(IProductService productService)
        {
            _productService = productService;
        }
        public int FeatureId { get; set; }
        public List<FeatureValue> FeatureValues { get; set; }
        [BindProperty]
        public FeatureValue FeatureValue { get; set; }
        public void OnGet(int id)
        {
            FeatureId = id;
            ViewData["FeatureTitle"] = _productService.GetFeatureById(id).FeatureTitle;
            FeatureValues = _productService.GetFeatureValues(id);
        }
        public IActionResult OnPostEditFeatureValue()
        {
            FeatureId = FeatureValue.FeatureId;
            ViewData["FeatureTitle"] = _productService.GetFeatureById(FeatureId).FeatureTitle;
            FeatureValues = _productService.GetFeatureValues(FeatureId);

            if (!ModelState.IsValid)
                return Page();

            FeatureValue editFeatureValue = _productService.GetFeatureValueById(FeatureValue.FeatureValueId);
            if (editFeatureValue != null)
            {
                editFeatureValue.ValueTitle = FeatureValue.ValueTitle;
                _productService.UpdateFeatureValue(editFeatureValue);
            }

            return RedirectToPage(new { FeatureId });
        }
        public IActionResult OnPostCreateFeatureValue()
        {
            FeatureId = FeatureValue.FeatureId;
            ViewData["FeatureTitle"] = _productService.GetFeatureById(FeatureId).FeatureTitle;
            FeatureValues = _productService.GetFeatureValues(FeatureId);

            if (!ModelState.IsValid)
                return Page();


            if (!string.IsNullOrWhiteSpace(FeatureValue.ValueTitle))
            {
                _productService.CreateFeatureValue(FeatureValue);
            }
            return RedirectToPage(new { FeatureId });
        }

        public IActionResult OnPostDeleteFeatureValue(int featureValueId)
        {
            FeatureValue value = _productService.GetFeatureValueById(featureValueId);
            FeatureId = value.FeatureId;
            if (value != null)
            {
                _productService.DeleteFeatureValue(value);
            }

            return RedirectToPage(new { FeatureId });
        }
    }
}
