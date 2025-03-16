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
using Microsoft.IdentityModel.Protocols.WsTrust;
using ShahrChap.DataLayer.Migrations;

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
                Value = nameof(g.GroupId)
            }).ToList();
        }

        public List<SelectListItem> GetSubGroupForManageProducts(int groupId)
        {
            return _context.ProductGroups.Where(g => g.ParentId == groupId).Select(g => new SelectListItem()
            {
                Text = g.GroupTitle,
                Value = nameof(g.GroupId)
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
            return _context.Products.Where(p => p.ParentId == null).Select(p => new ShowProductForAdminViewModel(p.ProductId, p.ProductTitle, p.Image)).ToList();
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
            //_context.Products.Update(product);
            UpdateProduct(product, null);
            _context.SaveChanges();
        }
        #endregion
        #region Feature
        public List<ProductFeature> GetProductFeatures(int productId)
        {
            return _context.ProductFeatures.Where(f => f.ProductId == productId).Include(feature => feature.Feature).ToList();
        }
        public List<Feature> GetAllFeatures()
        {
            return _context.Features.Include(f => f.FeatureValues).ToList();
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
        public List<int> ProductFeatureIds(int productId)
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
        #region SubProduct
        public List<ShowProductForAdminViewModel> GetSubProductForAdmin(int id)
        {
            return _context.Products.Where(p => p.ParentId == id).Select(p => new ShowProductForAdminViewModel(p.ProductId, p.ProductTitle, p.Image)).ToList();
        }
        #endregion
        #region FeatureValues
        public List<FeatureValue> GetAllFeatureValues(int parentId)
        {
            List<FeatureValue> Values = new List<FeatureValue>();
            List<ProductFeature> ProductFeatures = GetProductFeatures(parentId);
            foreach (var feature in ProductFeatures)
            {
                //List<Feature> features = .ToList();
                Values.AddRange(_context.FeatureValues.Where(f => f.FeatureId == feature.FeatureId).Include(f => f.Feature));
            }
            return Values;
        }
        public List<int> SubProductFeatureValueIds(int productId)
        {
            return _context.ProductFeatureValues
                .Where(p => p.ProductId == productId)
                .Select(p => p.FeatureValueId).ToList();
        }
        public void AddFeatureValuesToProduct(int productId, List<int> featureValues)
        {
            var featureMappings = _context.FeatureValues
            .Where(fv => featureValues.Contains(fv.FeatureValueId))
            .Select(fv => new { fv.FeatureValueId, fv.FeatureId })
            .ToList();

            for (int i = 0; i < featureMappings.Count; i++)
            {
                var productFeatureValue = new ProductFeatureValue
                {
                    ProductId = productId,
                    FeatureId = featureMappings[i].FeatureId,
                    FeatureValueId = featureMappings[i].FeatureValueId
                };
                _context.ProductFeatureValues.Add(productFeatureValue);
            }

            _context.SaveChanges();
        }
        public void UpdateFeatureValuesProduct(int productId, List<int> values)
        {
            _context.ProductFeatureValues
                .Where(p => p.ProductId == productId).ToList()
                .ForEach(p => _context.ProductFeatureValues.Remove(p));

            AddFeatureValuesToProduct(productId, values);
        }
        #endregion
        #region Pricing
        public bool AreCombinationsChanged(int productId, List<string> Combintations)
        {
            List<string> ProductCombinations = _context.ProductPrices.Where(p => p.ProductId == productId).Select(p => p.Combination).ToList();
            return !new HashSet<string>(ProductCombinations).SetEquals(Combintations);
        }
        public List<string> GetFeatureCombinations(int productId)
        {
            var productFeatureValues = _context.ProductFeatureValues
                .Where(pfv => pfv.ProductId == productId)
                .Include(pfv => pfv.Feature)
                .Include(pfv => pfv.FeatureValue)
                .ToList();

            // Group feature values by feature
            var featureValueLists = productFeatureValues
                .GroupBy(pfv => pfv.FeatureId)
                .Select(g => g.Select(pfv => pfv.FeatureValue.ValueTitle).ToList())
                .ToList();

            return GenerateCombinations(featureValueLists, 0, new List<string>());
        }
        private List<string> GenerateCombinations(List<List<string>> featureValues, int index, List<string> current)
        {
            if (index == featureValues.Count)
            {
                return new List<string> { string.Join(" - ", current) };
            }

            var combinations = new List<string>();
            foreach (var value in featureValues[index])
            {
                var newCurrent = new List<string>(current) { value };
                combinations.AddRange(GenerateCombinations(featureValues, index + 1, newCurrent));
            }
            return combinations;
        }
        public void AddProductPrices(int productId, List<ProductPrice> price)
        {
            DeleteProductPrices(productId);

            for (int i = 0; i < price.Count; i++)
            {
                _context.ProductPrices.Add(new ProductPrice
                {
                    ProductId = productId,
                    Combination = price[i].Combination,
                    Price = price[i].Price
                });
            }
            _context.SaveChanges();
        }
        public void UpdateProductPrices(int productId, List<ProductPrice> prices)
        {
            foreach (var price in prices)
            {
                var existingPrice = _context.ProductPrices
                    .Include(p => p.ServicePrices)
                    .FirstOrDefault(p => p.ProductPriceId == price.ProductPriceId);

                if (existingPrice != null)
                {
                    // Update ProductPrice fields
                    existingPrice.Price = price.Price;

                    foreach (var servicePrice in price.ServicePrices)
                    {
                        var existingServicePrice = existingPrice.ServicePrices
                            .FirstOrDefault(sp => sp.ProductServiceId == servicePrice.ProductServiceId);

                        if (existingServicePrice != null)
                        {
                            // Update existing service price
                            existingServicePrice.Price = servicePrice.Price;
                        }
                        else
                        {
                            // Add new service price if not found
                            existingPrice.ServicePrices.Add(servicePrice);
                        }
                    }

                    _context.ProductPrices.Update(existingPrice);
                }
            }
            _context.SaveChanges();
        }

        public void DeleteProductPrices(int productId)
        {
            _context.ProductPrices
                            .Where(p => p.ProductId == productId).ToList()
                            .ForEach(p => _context.ProductPrices.Remove(p));
        }
        public List<ProductPrice> GetProductPrices(int productId)
        {
            return _context.ProductPrices.Where(p => p.ProductId == productId).ToList();
        }
        public void AddServicePrices(List<ServicePrice> servicePrices)
        {
            //Change the function and delete the old prices
            foreach (var servicePrice in servicePrices)
            {
                var existingServicePrice = _context.ServicePrices
                    .FirstOrDefault(sp => sp.ProductPriceId == servicePrice.ProductPriceId &&
                                          sp.ProductServiceId == servicePrice.ProductServiceId);

                if (existingServicePrice != null)
                {
                    existingServicePrice.Price = servicePrice.Price; // Update price
                }
                else
                {
                    _context.ServicePrices.Add(servicePrice);
                }
            }
            _context.SaveChanges();
        }
        public List<ServicePrice> GetServicePricesForProduct(int productId)
        {
            List<ProductPrice> productPrices = GetProductPrices(productId);
            Product product = GetProductById(productId);
            List<ServicePrice> servicePrices = new List<ServicePrice>();

            for (int i = 0; i < productPrices.Count; i++)
            {
                List<ServicePrice> currentProductPrice = _context.ServicePrices
                    .Where(sp => sp.ProductPriceId == productPrices[i].ProductPriceId)
                    .ToList();

                if (!currentProductPrice.Any())
                {
                    currentProductPrice = _context.Services
                        .Where(s => s.ProductId == product.ParentId)
                        .Select(s => new ServicePrice
                        {
                            ProductPriceId = productPrices[i].ProductPriceId,
                            ProductServiceId = s.ServiceId,
                            Price = 0
                        }).ToList();
                }

                servicePrices.AddRange(currentProductPrice);
            }
            return servicePrices;
        }
        #endregion
    }
}
