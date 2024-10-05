using Microsoft.AspNetCore.Mvc;
using ShahrChap.Core.Services.Interfaces;

namespace ShahrChap.Web.ViewComponents
{
    public class ProductGroupComponent:ViewComponent
    {
        private IProductService _productService;
        public ProductGroupComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult) View("ProductGroup", _productService.GetAllGroups()));
        }
    }
}
