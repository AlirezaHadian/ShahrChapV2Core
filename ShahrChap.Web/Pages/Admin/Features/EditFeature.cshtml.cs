using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Pages.Admin.Features
{
    public class EditFeatureModel : PageModel
    {
        private IProductService _productService;
        public EditFeatureModel(IProductService productService)
        {
            _productService = productService;
        }
        [BindProperty]
        public Feature Feature { get; set; }
        public void OnGet(int id)
        {
            Feature = _productService.GetFeatureById(id);
        }
        public IActionResult OnPost()
        {
            if(!ModelState.IsValid)
                return Page();

            _productService.UpdateFeature(Feature);
            return RedirectToPage("Index");
        }
    }
}
