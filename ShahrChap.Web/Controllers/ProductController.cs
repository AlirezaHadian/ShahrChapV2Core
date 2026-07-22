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
        private IPermissionService _permissionService;
        public ProductController(IProductService productService, IOrderService orderService, ICartService cartService, IUserService userService, IPermissionService permissionService)
        {
            _productService = productService;
            _orderService = orderService;
            _cartService = cartService;
            _userService = userService;
            _permissionService = permissionService;
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
                return Json(new { success = false, message = "حداکثر 5 فایل مجاز است." });

            if (model.OrderFiles == null || !model.OrderFiles.Any())
                return Json(new { success = false, message = "لطفاً حداقل یک فایل انتخاب کنید." });

            var validator = new FileUploadValidatior();
            foreach (var file in model.OrderFiles)
            {
                var result = validator.Validate(file);
                if (!result.IsValid)
                    return Json(new { success = false, message = result.ErrorMessage });
            }

            try
            {
                string token = null;

                if (!User.Identity.IsAuthenticated)
                {
                    token = Request.Cookies["cart-token"];
                    if (token == null)
                    {
                        token = Guid.NewGuid().ToString();
                        Response.Cookies.Append("cart-token", token);
                    }

                    _cartService.CreateCartOrAddItem(
                        model.ProductId, model.OrderTitle, model.FeaturesCombination,
                        model.ServiceIds, model.OrderFiles, token: token);
                }
                else
                {
                    _cartService.CreateCartOrAddItem(
                        model.ProductId, model.OrderTitle, model.FeaturesCombination,
                        model.ServiceIds, model.OrderFiles, userName: User.Identity.Name);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطای غیرمنتظره در ثبت سفارش رخ داد. " + ex.Message });
            }
        }

        #region Comment
        //[HttpPost]
        //public IActionResult CreateComment(CreateCommentDto comment)
        //{
        //    if (!User.Identity.IsAuthenticated)
        //        return Forbid();

        //    int userId = _userService.GetUserIdWithUserName(User.Identity.Name);
        //    var (success, message) = _productService.CreateComment(comment, userId);

        //    if (!success)
        //        return BadRequest(message);
        //    //bool isAdmin = _permissionService.CheckPermission("مدیریت نظرات", User.Identity.Name);

        //    var tree = _productService.GetCommentsTree(comment.ProductID, userId, isAdmin);
        //    return PartialView("_CommentList", tree);
        //}
        //[HttpPost]
        //public IActionResult EditComment(EditCommentDto editComment)
        //{
        //    if (!User.Identity.IsAuthenticated)
        //        return Forbid();

        //    int userId = _userService.GetUserIdWithUserName(User.Identity.Name);
        //    var (success, message) = _productService.EditComment(editComment, userId);

        //    if (!success)
        //        return BadRequest(message);

        //    int productId = _productService.GetProductIdByCommentId(editComment.CommentID);
        //    bool isAdmin = _permissionService.CheckPermission("مدیریت نظرات", User.Identity.Name);
        //    var tree = _productService.GetCommentsTree(productId, userId, isAdmin);
        //    return PartialView("_CommentList", tree);
        //}
        //[HttpPost]
        //public IActionResult DeleteComment(int commentId, int productId)
        //{
        //    if (!User.Identity.IsAuthenticated)
        //        return Forbid();

        //    int userId = _userService.GetUserIdWithUserName(User.Identity.Name);
        //    bool isAdmin = _permissionService.CheckPermission("مدیریت نظرات", User.Identity.Name);

        //    _productService.DeleteComment(commentId, userId, isAdmin);

        //    var tree = _productService.GetCommentsTree(productId, userId, isAdmin);
        //    return PartialView("_CommentList", tree);
        //}
        #endregion
    }
}
