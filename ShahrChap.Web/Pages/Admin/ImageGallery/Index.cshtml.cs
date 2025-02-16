using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Product;

namespace ShahrChap.Web.Pages.Admin.ImageGallery
{
    public class IndexModel : PageModel
    {
        private IProductService _productService;
        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }
        public int ProductId { get; set; }
        public List<ProductGallery> GalleryList { get; set; }
        [BindProperty]
        public ProductGallery ImageGallery { get; set; }
        public void OnGet(int productId)
        {
            ProductId = productId;
            ViewData["ProductTitle"] = _productService.GetProductById(productId).ProductTitle;
            GalleryList = _productService.GetProductGalleryListById(productId);
        }

        public IActionResult OnPostCreateGallery(IFormFile imgProduct)
        {
            ProductId = ImageGallery.ProductId;
            ViewData["ProductTitle"] = _productService.GetProductById(ProductId);
            GalleryList = _productService.GetProductGalleryListById(ProductId);

            if (imgProduct != null)
                ModelState.Remove("ImageGallery.ImageName");

            if (!ModelState.IsValid)
                return Page();

            _productService.AddImageToProduct(ImageGallery, imgProduct);

            return RedirectToPage(new { ProductId});
        }

        public IActionResult OnPostDeleteGallery(int galleryId)
        {
            ProductGallery gallery = _productService.GetGalleryById(galleryId);
            ProductId = gallery.ProductId;
            if (gallery != null)
            {
                _productService.DeleteGallery(gallery);
            }

            return RedirectToPage(new { ProductId });
        }
    }
}
