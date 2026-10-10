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


        //public IViewComponentResult Invoke()
        //{
        //    var menu = _productService.GetMainMenu();

        //    return View(menu);
        //}
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menu = _productService.GetMainMenu();

            return await Task.FromResult((IViewComponentResult) View("ProductGroup", menu));
        }
    }
}
