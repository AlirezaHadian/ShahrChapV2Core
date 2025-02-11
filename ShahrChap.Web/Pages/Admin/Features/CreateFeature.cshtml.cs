using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Pages.Admin.Features
{
    public class CreateFeatureModel : PageModel
    {
        private IProductService _productService;
        public CreateFeatureModel(IProductService productService)
        {
            _productService = productService;
        }
        [BindProperty]
        public Feature Feature { get; set; }
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            if(!ModelState.IsValid)
                return Page();

            Feature.IsDelete = false;
            _productService.AddFeature(Feature);

            return RedirectToPage("Index");
        }
    }
}
