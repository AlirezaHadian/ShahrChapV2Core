using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShahrChap.Core.Convertors;
using ShahrChap.Core.DTOs.Products;
using ShahrChap.Core.Generators;
using ShahrChap.Core.Security;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Context;
using ShahrChap.DataLayer.Entities.Order;
using ShahrChap.DataLayer.Entities.Product;


namespace ShahrChap.Core.Services
{
    public class ProductService : IProductService
    {
        private ShahrChapContext _context;
        private IPermissionService _permissionService;
        public ProductService(ShahrChapContext context, IPermissionService permissionService)
        {
            _context = context;
            _permissionService = permissionService;
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
        #region Type
        public List<SelectListItem> GetTypes()
        {
            return _context.ProductTypes.Select(g => new SelectListItem()
            {
                Text = g.TypeTitle,
                Value = g.ProductTypeId.ToString()
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
            imgResizer.ResizeImage(imagePath, thumbPath, 250);
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
            return _context.Products.Where(p => p.ParentId == null).Include(p => p.ProductType).Select(p => new ShowProductForAdminViewModel(p.ProductId, p.ProductTitle, p.Image, p.IsDesignable)).ToList();
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
        public List<ShowProductListViewModel> GetProducts(int take = 0, string filter = "", int? parentId = null, int? subGroupId = null)
        {
            if (take == 0)
                take = 8;

            IQueryable<Product> result = _context.Products;

            if (!string.IsNullOrEmpty(filter))
            {
                result = result.Where(p => p.ProductTitle.Contains(filter));
            }

            if (parentId.HasValue)
            {
                result = result.Where(p => p.ParentId == parentId.Value);
            }
            else
            {
                result = result.Where(p => p.ParentId == null);
            }

            if (subGroupId.HasValue)
            {
                result = result.Where(p =>
                    p.SubGroupId == subGroupId.Value);
            }

            result = result.OrderByDescending(p => p.CreateDate);

            return result.Include(p => p.Group).Select(p => new ShowProductListViewModel()
            {
                ProductId = p.ProductId,
                ImageName = p.Image,
                ProductName = p.ProductTitle,
                GroupName = p.SubGroup.GroupTitle
            }).Take(take).ToList();
        }
        public List<HomeProductSectionViewModel> GetProductSections()
        {
            List<int> parentsGroupId = _context.ProductGroups
                .Where(g => g.ParentId == null)
                .Select(g => g.GroupId)
                .ToList();

            var groups = _context.ProductGroups
                .Where(g => g.ParentId.HasValue &&
                parentsGroupId.Contains(g.ParentId.Value))
                .Select(g => new
                {
                    Id = g.GroupId,
                    Title = g.GroupTitle
                }).ToList();

            var sections = new List<HomeProductSectionViewModel>();
            foreach (var group in groups)
            {
                var products = GetProducts(
                    take: 8,
                    subGroupId: group.Id
                    );

                if (products.Count == 0)
                    continue;

                sections.Add(new HomeProductSectionViewModel
                {
                    Title = group.Title,
                    Products = products
                });

            }
            return sections;
        }
        public Product GetProductForShow(int productId)
        {
            Product product = _context.Products.Include(p => p.ProductGalleries).FirstOrDefault(p => p.ProductId == productId);
            if (product != null && product.ParentId != null)
            {

            }
            return product;
        }
        public List<ShowProductListViewModel> GetSubProductForBox(int parentId)
        {
            List<ShowProductListViewModel> subProducts = _context.Products.Include(p => p.Group)
                .Where(p => p.ParentId == parentId)
                .Select(p => new ShowProductListViewModel()
                {
                    ProductId = p.ProductId,
                    ImageName = p.Image,
                    ProductName = p.ProductTitle,
                    GroupName = p.Group.GroupTitle
                }).ToList();

            return subProducts;
        }
        public string GetProductTitleById(int productId)
        {
            return _context.Products.Find(productId).ProductTitle;
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
        public string GetServiceTitlesByIdList(List<int> servicesIds)
        {
            var titles = _context.Services
                .Where(s => servicesIds.Contains(s.ServiceId))
                .Select(s => s.ServiceTitle)
                .ToList();

            return string.Join("-", titles);
        }
        public List<OrderDetailService> GetOrderDetailsServices(int orderDetailId)
        {
            return _context.OrderDetailServices
                    .Where(od => od.OrderDetailID == orderDetailId)
                    .ToList();
        }
        public string GetServiceTitleById(int serviceTitleId)
        {
            return _context.Services
    .Where(s => s.ServiceId == serviceTitleId)
    .Select(s => s.ServiceTitle)
    .FirstOrDefault();
        }

        public long CalculateServicePrice(int serviceId, int productId, string combination)
        {
            var productPrice = _context.ProductPrices
                .FirstOrDefault(p => p.ProductId == productId && p.Combination == combination);

            if (productPrice == null)
                throw new InvalidOperationException(
                    $"ترکیب ویژگی '{combination}' برای محصول {productId} قیمت گذاری نشده است");

            var servicePrice = _context.ServicePrices
                .FirstOrDefault(sp => sp.ProductServiceId == serviceId && sp.ProductPriceId == productPrice.ProductPriceId);

            if (servicePrice == null)
                throw new InvalidOperationException(
                    $"سرویس {serviceId} برای این ترکیب قیمت‌گذاری نشده است.");

            return servicePrice.Price;
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
            return _context.Products.Where(p => p.ParentId == id).Select(p => new ShowProductForAdminViewModel(p.ProductId, p.ProductTitle, p.Image, p.IsDesignable)).ToList();
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
            Product product = GetProductById(productId);
            foreach (var price in prices)
            {
                var existingPrice = _context.ProductPrices
                    .Include(p => p.DesignPrice)
                    .Include(p => p.ServicePrices)
                    .FirstOrDefault(p => p.ProductPriceId == price.ProductPriceId);

                if (existingPrice != null)
                {
                    // Update ProductPrice fields
                    existingPrice.Price = price.Price;
                    if (price.ServicePrices != null || price.ServicePrices.Any())
                    {
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
                    }

                    if (product.IsDesignable)
                    {
                        existingPrice.DesignPrice = price.DesignPrice;
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
            return _context.ProductPrices.Where(p => p.ProductId == productId).Include(p => p.DesignPrice).ToList();
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

        public ProductPriceViewModel GetCombinationPriceForShowProduct(int productId, string combination)
        {
            return _context.ProductPrices
                .Where(p => p.ProductId == productId && p.Combination == combination)
                .Select(p => new ProductPriceViewModel()
                {
                    ProductPriceId = p.ProductPriceId,
                    Price = p.Price
                })
                .FirstOrDefault();
        }

        public decimal GetServicePriceForShowProduct(int productPriceId, int serviceId)
        {
            return _context.ServicePrices
                .Where(s => s.ProductServiceId == serviceId && s.ProductPriceId == productPriceId)
                .Select(p => p.Price)
                .FirstOrDefault();
        }

        public decimal CalculatePrice(int productId, string combination, List<int> services)
        {

            ProductPriceViewModel productPrice = GetCombinationPriceForShowProduct(productId, combination);
            decimal totalPrice = 0;
            if (productPrice != null)
            {
                totalPrice += productPrice.Price;
                if (services != null)
                {
                    foreach (var service in services)
                    {
                        var servicePrice = GetServicePriceForShowProduct(productPrice.ProductPriceId, service);
                        totalPrice += servicePrice;
                    }
                }
            }
            return totalPrice;
        }
        public int GetProductPriceId(int productId, string combination)
        {
            return _context.ProductPrices.FirstOrDefault(p => p.ProductId == productId && p.Combination == combination).ProductPriceId;
        }

        #endregion
        #region Comments
        public List<ProductCommentViewModel> GetCommentsTree(int productId, int? currentUserId)
        {
            var allComments = _context.ProductComments
                .IgnoreQueryFilters()
                .Include(c => c.User)
                .Where(c => c.ProductID == productId)
                .OrderBy(c => c.CreateDate)
                .ToList();

            var userIds = allComments.Select(c => c.UserID).Distinct().ToList();
            var roleTitles = _permissionService.GetPrimaryRoleTitles(userIds);
            var authorNameById = allComments.ToDictionary(c => c.CommentID, c => c.User?.UserName ?? "کاربر");
            var commentById = allComments.ToDictionary(c => c.CommentID);

            int GetRootId(ProductComment c)
            {
                var current = c;
                while (current.ParentID.HasValue && commentById.ContainsKey(current.ParentID.Value))
                    current = commentById[current.ParentID.Value];
                return current.CommentID;
            }

            ProductCommentViewModel ToViewModel(ProductComment c, string inReplyTo = null)
            {
                bool isOwner = currentUserId.HasValue && c.UserID == currentUserId.Value;
                return new ProductCommentViewModel
                {
                    CommentID = c.CommentID,
                    UserFullName = c.User?.UserName ?? "کاربر",
                    RoleTitle = roleTitles.TryGetValue(c.UserID, out var rt) ? rt : null,
                    Text = c.Text,
                    CreateDate = c.CreateDate,
                    IsDeleted = c.IsDeleted,
                    IsEdited = c.IsEdited,
                    IsOwner = !c.IsDeleted && isOwner,
                    InReplyToUserName = inReplyTo
                };
            }

            var roots = allComments.Where(c => !c.ParentID.HasValue).ToList();
            var result = new List<ProductCommentViewModel>();

            foreach (var root in roots)
            {
                var rootVm = ToViewModel(root);

                var descendants = allComments
                    .Where(c => c.CommentID != root.CommentID && GetRootId(c) == root.CommentID)
                    .OrderBy(c => c.CreateDate)
                    .ToList();

                foreach (var d in descendants)
                {
                    string inReplyTo = (d.ParentID.HasValue && d.ParentID.Value != root.CommentID
                        && authorNameById.ContainsKey(d.ParentID.Value))
                        ? authorNameById[d.ParentID.Value]
                        : null;

                    rootVm.Replies.Add(ToViewModel(d, inReplyTo));
                }

                result.Add(rootVm);
            }

            return result;
        }
        public (bool Success, string Message) CreateComment(CreateCommentDto dto, int userId)
        {
            if (string.IsNullOrWhiteSpace(dto.Text))
                return (false, "متن نظر نمی‌تواند خالی باشد.");

            if (dto.Text.Length > 700)
                return (false, "متن نظر بیش از حد مجاز است.");

            if (dto.ParentID.HasValue)
            {
                bool parentExists = _context.ProductComments
                    .Any(c => c.CommentID == dto.ParentID.Value && c.ProductID == dto.ProductID && !c.IsDeleted);

                if (!parentExists)
                    return (false, "دیدگاه مورد نظر برای پاسخ یافت نشد.");
            }

            _context.ProductComments.Add(new ProductComment
            {
                ProductID = dto.ProductID,
                UserID = userId,
                ParentID = dto.ParentID,
                Text = dto.Text.Trim(),
                CreateDate = DateTime.Now
            });

            _context.SaveChanges();
            return (true, "دیدگاه شما ثبت شد.");
        }
        public (bool Success, string Message) EditComment(EditCommentDto dto, int userId)
        {
            var comment = _context.ProductComments.Find(dto.CommentID);

            if (comment == null || comment.IsDeleted)
                return (false, "دیدگاه مورد نظر یافت نشد.");

            if (comment.UserID != userId)
                return (false, "شما اجازه‌ی ویرایش این دیدگاه را ندارید.");

            if (string.IsNullOrWhiteSpace(dto.Text))
                return (false, "متن نظر نمی‌تواند خالی باشد.");

            if (dto.Text.Length > 700)
                return (false, "متن نظر بیش از حد مجاز است.");

            comment.Text = dto.Text.Trim();
            comment.IsEdited = true;
            comment.EditDate = DateTime.Now;

            _context.ProductComments.Update(comment);
            _context.SaveChanges();

            return (true, "دیدگاه با موفقیت ویرایش شد.");
        }
        public bool DeleteComment(int commentId, int userId)
        {
            var comment = _context.ProductComments.Find(commentId);
            if (comment == null) return false;

            if (comment.UserID != userId) return false;

            comment.IsDeleted = true;
            _context.ProductComments.Update(comment);
            _context.SaveChanges();
            return true;
        }

        public int GetProductIdByCommentId(int commentId)
        {
            return _context.ProductComments
                .Where(c => c.CommentID == commentId)
                .Select(c => c.ProductID)
                .FirstOrDefault();
        }
        #endregion
    }
}