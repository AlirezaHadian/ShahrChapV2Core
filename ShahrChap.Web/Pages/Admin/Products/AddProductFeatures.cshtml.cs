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
        public List<Feature> Features { get; set; }
        [BindProperty]
        public Feature Feature { get; set; }

        public void OnGet(int id)
        {
            Features = _productService.GetAllFeatures();
        }
        public void OnPost()
        {

        }
    }
}
