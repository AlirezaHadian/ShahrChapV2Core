using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Pages.Admin.Products
{
    public class EditProductModel : PageModel
    {
        private IProductService _productService;
        public EditProductModel(IProductService productService)
        {
            _productService = productService;
        }
        [BindProperty]
        public Product Product { get; set; }
        public void OnGet(int id)
        {
            Product = _productService.GetProductById(id);
            var groups = _productService.GetGroupForManageProducts();
            ViewData["Groups"] = new SelectList(groups, "Value", "Text", Product.GroupId);

            var subGroups = _productService.GetSubGroupForManageProducts(Product.GroupId);
            ViewData["SubGroups"] = new SelectList(subGroups, "Value", "Text", Product.SubGroupId);

        }

        public IActionResult OnPost(IFormFile? imgProduct)
        {
            if (!ModelState.IsValid)
            {
                Product = _productService.GetProductById(Product.ProductId);

                var groups = _productService.GetGroupForManageProducts();
                ViewData["Groups"] = new SelectList(groups, "Value", "Text", Product.GroupId);

                var subGroups = _productService.GetSubGroupForManageProducts(Product.GroupId);
                ViewData["SubGroups"] = new SelectList(groups, "Value", "Text", Product.SubGroupId);
                return Page();
            }

            _productService.UpdateProduct(Product, imgProduct);

            return RedirectToPage("Index");
        }

        public JsonResult OnGetJson(int id)
        {
            var subGroup = _productService.GetSubGroupForManageProducts(id);
            SelectList list = new SelectList(subGroup, "Value", "Text");
            return new JsonResult(list);
        }
    }
}
