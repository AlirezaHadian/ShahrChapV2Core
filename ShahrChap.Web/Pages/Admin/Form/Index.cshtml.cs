using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Enums;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;
using ShahrChap.DataLayer.Entities.Product.Form;

namespace ShahrChap.Web.Pages.Admin.Form
{
    public class IndexModel(IProductService productService) : PageModel
    {
        private IProductService _productService = productService;

        public Product Product { get; set; }
        public List<ProductForm> Forms { get; set; }
        public bool CheckNewForm { get; set; }
        public FormCreationState State { get; set; }
        public void OnGet(int id)
        {
            Product = _productService.GetProductById(id);
            Forms = _productService.GetProductForms(id);
            State = _productService.GetPendingForms(id);

        }
    }
}
