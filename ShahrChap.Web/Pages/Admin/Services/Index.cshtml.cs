using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Pages.Admin.Features
{
    public class IndexModel : PageModel
    {
        private IProductService _productService;
        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }
        public List<Feature> Features { get; set; }
        public void OnGet()
        {
            Features = _productService.GetAllFeatures();
        }
    }
}
