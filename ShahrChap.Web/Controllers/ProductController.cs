using Microsoft.AspNetCore.Mvc;
using ShahrChap.Core.DTOs.Order;
using ShahrChap.Core.DTOs.Products;
using ShahrChap.Core.Security;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Cart;
using ShahrChap.DataLayer.Entities.Product;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Pkcs;

namespace ShahrChap.Web.Controllers
{
    public class ProductController : Controller
    {
        private IProductService _productService;
        private IOrderService _orderService;
        private ICartService _cartService;
        private IUserService _userService;
        public ProductController(IProductService productService, IOrderService orderService, ICartService cartService)
        {
            _productService = productService;
            _orderService = orderService;
            _cartService = cartService;
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
                string combination = string.Join(" - ", options.Values);
                decimal totalPrice = _productService.CalculatePrice(productId, combination, services);
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
            if (model.OrderFiles.Count > 5)
            {
                return Json(new { success = false, message = "حداکثر 5 فایل مجاز است." });
            }

            if (model.OrderFiles == null || !model.OrderFiles.Any())
            {
                return Json(new { success = false, message = "لطفاً حداقل یک فایل انتخاب کنید." });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".zip", ".rar", ".psd", ".tiff" };
            long maxFileSize = 50 * 1024 * 1024;

            var validator = new FileUploadValidatior();
            foreach (var file in model.OrderFiles)
            {
                var result = validator.Validate(file);
                if (!result.IsValid)
                    return Json(new { success = false, message = result.ErrorMessage });
            }

            try
            {
                Cart cart = new Cart();
                string token = null;
                if (!User.Identity.IsAuthenticated)
                {
                    token = Request.Cookies["cart-token"];
                    if (token == null)
                    {
                        token = Guid.NewGuid().ToString();
                        Response.Cookies.Append("cart-token", token);
                    }
                    cart = _cartService.GetUserActiveCart(token: token);

                    _cartService.CreateCartOrAddItem(model.ProductId, model.FeaturesCombination, model.ServiceIds, token: token, cart: cart);
                }
                else
                {
                    cart = _cartService.GetUserActiveCart(User.Identity.Name);

                    _cartService.CreateCartOrAddItem(model.ProductId, model.FeaturesCombination, model.ServiceIds, User.Identity.Name, cart: cart);

                }
                CreateOrderDetailDto orderDto = new CreateOrderDetailDto()
                {
                    ProductId = model.ProductId,
                    ProductTitle = _productService.GetProductTitleById(model.ProductId),
                    OrderTitle = model.OrderTitle,
                    ServicesId = model.ServiceIds,
                    FeaturesCombination = model.FeaturesCombination,
                    Files = model.OrderFiles
                };
                if (User.Identity.IsAuthenticated)
                    await _orderService.CreateOrderAsync(orderDto, User.Identity.Name, null);
                else
                    await _orderService.CreateOrderAsync(orderDto, null, token);


                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "خطای غیرمنتظره در ثبت سفارش رخ داد." +
                    ex
                });
            }
        }
    }
}
