using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Pages.Admin.Products
{
    public class CreateProductModel : PageModel
    {
        private IProductService _productService;
        public CreateProductModel(IProductService productService)
        {
            _productService = productService;
        }
        [BindProperty]
        public Product Product { get; set; }
        public void OnGet()
        {
            var groups = _productService.GetGroupForManageProducts();
            ViewData["Groups"] = new SelectList(groups, "Value", "Text");
        }
        //Change the view page default image location
        public IActionResult OnPost(IFormFile? imgProduct)
        {
            if (!ModelState.IsValid)
            {
                var groups = _productService.GetGroupForManageProducts();
                ViewData["Groups"] = new SelectList(groups, "Value", "Text");
                
                return Page();
            }

            _productService.AddProudct(Product, imgProduct);

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
