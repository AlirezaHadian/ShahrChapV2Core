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
            if (product.ParentId == null)
            {
                List<ShowProductListViewModel> subProducts = _productService.GetSubProductForBox(product.ProductId);
                var model = new ParentProductForShowViewModel(product, subProducts);
                return View("ParentProduct", model);
            }
            else
            {
                List<ProductFeature> features = _productService.GetProductFeatures(product.ParentId.Value);
                List<FeatureValue> featureValues = _productService.GetAllFeatureValues(product.ParentId.Value);
                List<Service> services = _productService.GetProductServices(product.ParentId.Value);
                ViewBag.SelectedFeatureValues = _productService.SubProductFeatureValueIds(id);
                var model = new SubProductForShowViewMode(product, features, featureValues, services);
                return View("SubProduct", model);
            }
        }

        [HttpPost]
        public IActionResult CalculatePrice(int productId, Dictionary<string, string> options, List<int> services)
        {
            try
            {
                decimal totalPrice = 0;
                string combination = string.Join(" - ", options.Values);
                ProductPriceViewModel productPrice = _productService.GetCombinationPriceForShowProduct(productId, combination);
                if (productPrice != null)
                {
                    totalPrice += productPrice.Price;
                    foreach (var service in services)
                    {
                        var servicePrice = _productService.GetServicePriceForShowProduct(productPrice.ProductPriceId, service);
                        totalPrice += servicePrice;
                    }
                }
                return Json(new { success = true, price = totalPrice });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
    }
}
