using Microsoft.AspNetCore.Mvc.Rendering;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Context;
using ShahrChap.DataLayer.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ShahrChap.Core.Generators;
using ShahrChap.Core.DTOs.Products;
using ShahrChap.Core.Convertors;
using ShahrChap.Core.Security;
using ShahrChap.DataLayer.Entities.User;

namespace ShahrChap.Core.Services
{
    public class ProductService : IProductService
    {
        private ShahrChapContext _context;
        public ProductService(ShahrChapContext context)
        {
            _context = context;
        }
        #region Group
        public List<ProductGroup> GetAllGroups()
        {
            return _context.ProductGroups.ToList();
        }

        public List<SelectListItem> GetGroupForManageProducts()
        {
            return _context.ProductGroups.Where(g => g.ParentId == null).Select(g => new SelectListItem()
            {
                Text = g.GroupTitle,
                Value = g.GroupId.ToString()
            }).ToList();
        }

        public List<SelectListItem> GetSubGroupForManageProducts(int groupId)
        {
            return _context.ProductGroups.Where(g => g.ParentId == groupId).Select(g => new SelectListItem()
            {
                Text = g.GroupTitle,
                Value = g.GroupId.ToString()
            }).ToList();
        }
        #endregion
        #region Product
        public int AddProudct(Product product, IFormFile imgProduct)
        {
            product.CreateDate = DateTime.Now;
            product.Image = "NoImage.jpg";
            //Check image
            if (product.SubGroupId == 0)
                product.SubGroupId = null;

            if (imgProduct != null && imgProduct.IsImage())
            {
                product.Image = AddProductImage(imgProduct);
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            return product.ProductId;
        }

        public string AddProductImage(IFormFile productImage)
        {
            string productImageName = NameGenerator.GenerateUniqCode() + Path.GetExtension(productImage.FileName);
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/product/image", productImageName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                productImage.CopyTo(stream);
            }

            ImageConvertor imgResizer = new ImageConvertor();
            string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/product/thumb", productImageName);
            imgResizer.ResizeImage(imagePath, thumbPath, 150);
            return productImageName;
        }

        public void DeleteProductImage(string currentProductName)
        {
            //ToDo: Delete the thumb and main image
            if (currentProductName != "NoImage.jpg")
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/product/image",
                    currentProductName);
                string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/product/thumb",
                    currentProductName);
                if (File.Exists(imagePath))
                    File.Delete(imagePath);

                if (File.Exists(thumbPath))
                    File.Delete(thumbPath);
            }
        }

        public List<ShowProductForAdminViewModel> GetProductsForAdmin()
        {
            return _context.Products.Select(p => new ShowProductForAdminViewModel(p.ProductId, p.ProductTitle, p.Image)).ToList();
        }

        public Product GetProductById(int productId)
        {
            return _context.Products.Find(productId);
        }

        public void UpdateProduct(Product product, IFormFile imgProduct)
        {
            //Check update
            if (product.SubGroupId == 0)
                product.SubGroupId = null;

            if (imgProduct != null && imgProduct.IsImage())
            {
                if (product.Image != null)
                {
                    //Delete old image
                    DeleteProductImage(product.Image);
                }
                product.Image = AddProductImage(imgProduct);
            }

            _context.Products.Update(product);
            _context.SaveChanges();
        }
        public void DeleteProduct(Product product)
        {
            product.IsDelete = true;
            _context.Products.Update(product);
            _context.SaveChanges();
            //UpdateProduct(product);
        }
        #endregion
        #region Feature
        public List<ProductFeature> GetProductFeatures(int productId)
        {
            return _context.ProductFeatures.Where(f => f.ProductId == productId).Include(feature => feature.Feature).ToList();
        }
        public List<Feature> GetAllFeatures()
        {
            return _context.Features.ToList();
        }

        public Feature GetFeatureById(int featureId)
        {
            return _context.Features.Find(featureId);
        }

        public int CreateFeature(Feature feature)
        {
            _context.Features.Add(feature);
            _context.SaveChanges();
            return feature.FeatureId;
        }
        public void UpdateFeature(Feature feature)
        {
            _context.Features.Update(feature);
            _context.SaveChanges();
        }

        public void DeleteFeature(Feature feature)
        {
            feature.IsDelete = true;
            UpdateFeature(feature);
        }
        public void AddFeaturesToProduct(int productId, List<int> features)
        {
            for (int i = 0; i < features.Count(); i++)
            {
                _context.ProductFeatures.Add(new ProductFeature()
                {
                    ProductId = productId,
                    FeatureId = features[i]
                });
            }
            _context.SaveChanges();
        }
        public void UpdateFeaturesProduct(int productId, List<int> features)
        {
            _context.ProductFeatures
                .Where(p => p.ProductId == productId).ToList()
                .ForEach(p => _context.ProductFeatures.Remove(p));
            AddFeaturesToProduct(productId, features);
        }

        public List<int> ProductFeatures(int productId)
        {
            return _context.ProductFeatures
                .Where(p => p.ProductId == productId)
                .Select(p => p.FeatureId).ToList();
        }
        public List<FeatureValue> GetFeatureValues(int featureId)
        {
            return _context.FeatureValues.Where(f => f.FeatureId == featureId).ToList();
        }
        public int CreateFeatureValue(FeatureValue value)
        {
            _context.FeatureValues.Add(value);
            _context.SaveChanges();
            return value.FeatureValueId;
        }
        public FeatureValue GetFeatureValueById(int valueId)
        {
            return _context.FeatureValues.Find(valueId);
        }

        public void UpdateFeatureValue(FeatureValue value)
        {
            _context.FeatureValues.Update(value);
            _context.SaveChanges();
        }

        public void DeleteFeatureValue(FeatureValue value)
        {
            value.IsDelete = true;
            UpdateFeatureValue(value);
        }
        #endregion
        #region Service
        public List<Service> GetAllServices()
        {
            return _context.Services.ToList();
        }

        public List<Service> GetProductServices(int productId)
        {
            return _context.Services.Where(f => f.ProductId == productId).ToList();
        }

        public int CreateService(Service service)
        {
            _context.Services.Add(service);
            _context.SaveChanges();
            return service.ServiceId;
        }

        public Service GetServiceById(int serviceId)
        {
            return _context.Services.Find(serviceId);
        }

        public void UpdateService(Service service)
        {
            _context.Services.Update(service);
            _context.SaveChanges();
        }

        public void DeleteService(Service service)
        {
            service.IsDelete = true;
            UpdateService(service);
        }
        #endregion
        #region Product Gallery
        public List<ProductGallery> GetProductGalleryListById(int productId)
        {
            return _context.ProductGalleries.Where(p => p.ProductId == productId).ToList();
        }

        public int AddImageToProduct(ProductGallery gallery, IFormFile imgGallery)
        {
            if (imgGallery != null && imgGallery.IsImage())
            {
                gallery.ImageName = AddImageToProductGallery(imgGallery);
            }
            _context.ProductGalleries.Add(gallery);
            _context.SaveChanges();
            return gallery.ProductGalleryId;
        }

        public ProductGallery GetGalleryById(int galleryId)
        {
            return _context.ProductGalleries.Find(galleryId);
        }

        public void UpdateGallery(ProductGallery gallery, IFormFile imgGallery)
        {
            if (imgGallery != null && imgGallery.IsImage())
            {
                if (gallery.ImageName != null)
                {
                    //Delete old image
                    DeleteProductImage(gallery.ImageName);
                }
                gallery.ImageName = AddProductImage(imgGallery);
            }
            _context.ProductGalleries.Update(gallery);
            _context.SaveChanges();
        }

        public void DeleteGallery(ProductGallery gallery)
        {
            DeleteGalleryImage(gallery.ImageName);
            _context.ProductGalleries.Remove(gallery);
            _context.SaveChanges();
        }

        public string AddImageToProductGallery(IFormFile imageGallery)
        {
            string productGalleryName = NameGenerator.GenerateUniqCode() + Path.GetExtension(imageGallery.FileName);
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/product/image", productGalleryName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                imageGallery.CopyTo(stream);
            }

            ImageConvertor imgResizer = new ImageConvertor();
            string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/product/thumb", productGalleryName);
            imgResizer.ResizeImage(imagePath, thumbPath, 150);
            return productGalleryName;
        }

        public void DeleteGalleryImage(string currentGalleryName)
        {
            if (currentGalleryName != null)
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/product/image",
                    currentGalleryName);
                string thumbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/product/thumb",
                    currentGalleryName);
                if (File.Exists(imagePath))
                    File.Delete(imagePath);

                if (File.Exists(thumbPath))
                    File.Delete(thumbPath);
            }
        }
        #endregion
    }
}
