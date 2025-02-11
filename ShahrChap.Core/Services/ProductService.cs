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
        #endregion
        #region Feature
        public List<ProductFeature> GetProductFeatures(int productId)
        {
            return _context.ProductFeatures.Where(f=> f.ProductId == productId).Include(feature=> feature.Feature).ToList();
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
    }
}
