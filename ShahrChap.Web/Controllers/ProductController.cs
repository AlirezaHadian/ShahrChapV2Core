using Microsoft.AspNetCore.Mvc;
using ShahrChap.Core.DTOs.Products;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Controllers
{
    public class ProductController : Controller
    {
        private IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("ShowProduct/{id}")]
        public IActionResult ShowProduct(int id)
        {
            Product product = _productService.GetProductForShow(id);
            if(product.ParentId == null)
            {
                List<ShowProductListViewModel> subProducts = _productService.GetSubProductForBox(product.ProductId);
                var model = new ParentProductForShowViewModel(product, subProducts);
                return View("ParentProduct", model);
            }
            else
            {
                return View("SubProduct", product);
            }
        }
    }
}
