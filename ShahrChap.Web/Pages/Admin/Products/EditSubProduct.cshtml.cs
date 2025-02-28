using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Pages.Admin.Products
{
    public class EditSubProductModel : PageModel
    {
        private IProductService _productService;
        public EditSubProductModel(IProductService productService)
        {
            _productService = productService;
        }
        [BindProperty]
        public Product SubProduct { get; set; }
        public Product Parent { get; set; }
        public void OnGet(int id)
        {
            SubProduct = _productService.GetProductById(id);
            Parent = _productService.GetProductById((int)SubProduct.ParentId);

            if (Parent.SubGroupId != null)
            {
                var groups = _productService.GetSubGroupForManageProducts(Parent.SubGroupId.Value);
                ViewData["Groups"] = new SelectList(groups, "Value", "Text", SubProduct.SubGroupId);
            }
            else
            {
                SubProduct.GroupId = Parent.GroupId;
            }

        }

        public IActionResult OnPost(IFormFile? imgProduct)
        {
            Parent = _productService.GetProductById((int)SubProduct.ParentId);

            if (!ModelState.IsValid)
            {
                if (Parent.SubGroupId != null)
                {
                    var groups = _productService.GetSubGroupForManageProducts(Parent.SubGroupId.Value);
                    ViewData["Groups"] = new SelectList(groups, "Value", "Text", SubProduct.GroupId);
                }
                else
                {
                    SubProduct.GroupId = Parent.GroupId;
                }

                return Page();
            }

            _productService.UpdateProduct(SubProduct, imgProduct);

            return RedirectToPage("IndexSubProduct", new { id = SubProduct.ParentId });
        }
    }
}
