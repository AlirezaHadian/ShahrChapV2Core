using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Services;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;
using ShahrChap.DataLayer.Entities.User;

namespace ShahrChap.Web.Pages.Admin.Features
{
    public class DeleteFeatureModel : PageModel
    {
        private IProductService _productService;
        public DeleteFeatureModel(IProductService productService)
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
            _productService.DeleteFeature(Feature);
            return RedirectToPage("Index");
        }
    }
}
