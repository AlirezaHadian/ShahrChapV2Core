using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Pages.Admin.Products
{
    public class AddProductServiceModel : PageModel
    {
        private IProductService _productService;
        public AddProductServiceModel(IProductService productService)
        {
            _productService = productService;
        }
        [BindProperty(SupportsGet = true)]
        public int ProductId { get; set; }
        public List<Service> ProductServices { get; set; }
        [BindProperty]
        public Service Service { get; set; }
        public void OnGet(int id)
        {
            ProductId = id;
            ViewData["ProductTitle"] = _productService.GetProductById(id).ProductTitle;
            ProductServices = _productService.GetProductServices(id);
        }

        public IActionResult OnPostCreateService()
        {
            ProductId = Service.ProductId;
            ViewData["ProductTitle"] = _productService.GetProductById(ProductId).ProductTitle;
            ProductServices = _productService.GetProductServices(ProductId);

            //Model state check
            if (!ModelState.IsValid)
                return Page();

            if (!string.IsNullOrWhiteSpace(Service.ServiceTitle))
            {
                //Service.ProductId = ProductId;
                Service.IsDelete = false;
                _productService.CreateService(Service);
            }
            return RedirectToPage(new {ProductId});
        }
        
        public IActionResult OnPostDeleteService(int serviceId)
        {
            Service service = _productService.GetServiceById(serviceId);
            ProductId = service.ProductId;
            if (service != null)
            {
                _productService.DeleteService(service);
            }

            return RedirectToPage(new { ProductId });
        }
    }
}
