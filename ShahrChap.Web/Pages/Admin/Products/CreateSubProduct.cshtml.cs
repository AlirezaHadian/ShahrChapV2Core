using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Pages.Admin.Products
{
    public class CreateSubProductModel : PageModel
    {
        private IProductService _productService;
        public CreateSubProductModel(IProductService productService)
        {
            _productService = productService;
        }

        [BindProperty]
        public Product SubProduct { get; set; }
        public Product Parent {  get; set; }
        public void OnGet(int id)
        {
            SubProduct = new Product();
            Parent = _productService.GetProductById(id);
            if (Parent.SubGroupId != null)
            {
                var groups = _productService.GetSubGroupForManageProducts(Parent.SubGroupId.Value);
                ViewData["Groups"] = new SelectList(groups, "Value", "Text");
            }
            else
            {
                SubProduct.GroupId = Parent.GroupId;
            }

            SubProduct.ParentId = id;

        }
        public IActionResult OnPost(IFormFile? imgProduct)
        {
            Parent = _productService.GetProductById((int)SubProduct.ParentId);
            if (!ModelState.IsValid)
            {
                var groups = _productService.GetSubGroupForManageProducts(Parent.SubGroupId.Value);
                ViewData["Groups"] = new SelectList(groups, "Value", "Text");

                return Page();
            }

            SubProduct.ProductTypeId = Parent.ProductTypeId;

            _productService.AddProudct(SubProduct, imgProduct);

            return RedirectToPage("IndexSubProduct", new { id = SubProduct.ParentId });
        }
    }
}