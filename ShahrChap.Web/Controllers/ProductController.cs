using Microsoft.AspNetCore.Mvc;
using ShahrChap.Core.DTOs.Products;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;
using System.Security.Cryptography.Pkcs;

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
                ViewBag.IsUserLoggedIn = User.Identity.IsAuthenticated;
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

        [HttpPost]
        public async Task<ActionResult> SubmitFinalOrder(FinalOrderViewModel model)
        {
            if (model.OrderFiles == null || !model.OrderFiles.Any())
            {
                return Json(new { success = false, message = "لطفاً حداقل یک فایل انتخاب کنید." });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".zip", ".rar", ".psd", ".tiff" };
            long maxFileSize = 50 * 1024 * 1024;

            foreach (var file in model.OrderFiles)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                    return Json(new { success = false, message = $"پسوند فایل {file.FileName} مجاز نیست." });

                if (file.Length > maxFileSize)
                    return Json(new { success = false, message = $"فایل {file.FileName} بزرگتر از حد مجاز (۵۰ مگابایت) است." });

            }

            try
            {
                // ۳. ذخیره فایل‌ها در پوشه موقت یا اصلی
                // ۴. ثبت در دیتابیس (سفارش و آیتم‌های آن)
                // ۵. انتقال به سبد خرید (مثلاً ذخیره در کوکی یا دیتابیس)

                // مثال از عملیات نهایی:
                // _orderService.CreateOrder(model); 

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطای غیرمنتظره در ثبت سفارش رخ داد." });
            }
        }
    }
}
