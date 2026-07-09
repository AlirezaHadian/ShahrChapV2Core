using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShahrChap.Core.DTOs;
using ShahrChap.Core.DTOs.Cart;
using ShahrChap.Core.DTOs.Order;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Context;
using ShahrChap.DataLayer.Entities.Address;
using ShahrChap.DataLayer.Entities.Cart;
using ShahrChap.DataLayer.Entities.Order;
using ShahrChap.DataLayer.Entities.User;
using ShahrChap.DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.Services
{
    public class CartService : ICartService
    {
        private ShahrChapContext _context;
        private IUserService _userService;
        private IProductService _productService;
        private IOrderService _orderService;
        private IFileStorageService _fileStorage;
        public CartService(ShahrChapContext context, IUserService userService, IProductService productService, IOrderService orderService, IFileStorageService fileStorage)
        {
            _context = context;
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
            _fileStorage = fileStorage;
        }
        #region Cart
        public Cart GetUserActiveCart(string userName = null, string token = null)
        {
            Cart cart = new Cart();
            if (userName != null)
            {
                int userId = _userService.GetUserIdWithUserName(userName);
                cart = _context.Carts.Include(ci => ci.CartItems).FirstOrDefault(c => c.UserID == userId);
            }
            if (token != null)
            {
                cart = _context.Carts.Include(ci => ci.CartItems).FirstOrDefault(c => c.CartToken == token);
            }
            return cart;
        }

        public Cart CreateCartOrAddItem(int productId, string orderTitle, string featuresCombination,
    List<int> serviceIds, List<IFormFile> files, string userName = null, string token = null)
        {
            var cart = GetUserActiveCart(userName, token);

            if (cart == null)
            {
                cart = new Cart { CreateDate = DateTime.Now };
                if (userName != null) cart.UserID = _userService.GetUserIdWithUserName(userName);
                if (token != null) cart.CartToken = token;
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            string productName = _productService.GetProductTitleById(productId);
            long calculatedPrice = (long)_productService.CalculatePrice(productId, featuresCombination, serviceIds);

            var cartItem = new CartItem
            {
                CartID = cart.CartID,
                ProductID = productId,
                ProductTitle = productName,
                OrderTitle = orderTitle,
                FeaturesCombination = featuresCombination,
                CalculatedPrice = calculatedPrice,
                CartItemServices = CreateCartItemServices(serviceIds, productId, featuresCombination)
            };

            _context.CartItems.Add(cartItem);
            _context.SaveChanges();

            foreach(var item in files)
            {
                var file = new CartItemFile
                {
                    CartItemID = cartItem.CartItemID,
                    FileName = _fileStorage.SaveTempFile(cartItem.CartItemID, item),
                    OriginalFileName = item.FileName,
                    UploadDate = DateTime.Now
                };
                _context.CartItemFiles.Add(file);
            }
            _context.SaveChanges();

            return cart;
        }

        private List<CartItemService> CreateCartItemServices(List<int>? serviceIds, int productId, string combination)
        {
            if (serviceIds == null || !serviceIds.Any())
                return new List<CartItemService>();

            return serviceIds.Select(id => new CartItemService
            {
                ServiceID = id,
                ServiceTitle = _productService.GetServiceTitleById(id), // متد رو خودت باید داشته باشی/بسازی
                ServicePrice = _productService.CalculateServicePrice(id, productId, combination) // snapshot قیمت
            }).ToList();
        }
        public CartForPopoverViewModel ShowCartForPopover(string userName = null, string token = null)
        {
            var cart = GetUserActiveCart(userName, token);

            if (cart == null || !cart.CartItems.Any())
                return new CartForPopoverViewModel();

            var productIds = cart.CartItems.Select(c => c.ProductID).ToList();

            var products = _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .Select(p => new { p.ProductId, p.ProductTitle, p.Image })
                .ToList();

            var viewModel = new CartForPopoverViewModel
            {
                CartItems = cart.CartItems.Select(c =>
                {
                    var product = products.FirstOrDefault(p => p.ProductId == c.ProductID);
                    return new CartItemForPopoverViewModel
                    {
                        ProductId = c.ProductID,
                        ProductTitle = product?.ProductTitle ?? "محصول نامشخص",
                        ImageName = product?.Image ?? "default.jpg",
                        Price = c.CalculatedPrice
                    };
                }).ToList()
            };

            return viewModel;
        }
        public List<CartItem> GetUserCartItems(int cartId)
        {
            return _context.CartItems.Where(c => c.CartID == cartId).ToList();
        }

        public CartDetailsViewModel ShowCart(string userName = null, string token = null)
        {
            var cart = GetUserActiveCart(userName, token);
            var cartDetails = new CartDetailsViewModel { Cart = cart };

            if (userName != null)
                cartDetails.UserAddresses = _userService.GetUserAdresses(userName)
                    .Select(i => new AddressForCartViewModel
                    {
                        UserAddressId = i.AddressId,
                        FullAddress = i.FullAddress + "، پلاک" + i.HouseNumber,
                        AddressTitle = i.AddressTitle
                    }).ToList();

            if (cart == null) return cartDetails;

            var items = _context.CartItems
                .Include(c => c.CartItemServices)
                .Include(c => c.CartItemFiles)
                .Where(c => c.CartID == cart.CartID)
                .ToList();

            foreach (var item in items)
            {
                var featureTitles = _context.ProductFeatureValues
                    .Where(pfv => pfv.ProductId == item.ProductID)
                    .Select(pfv => pfv.Feature.FeatureTitle)
                    .Distinct()
                    .ToList();

                cartDetails.Items.Add(new CartItemViewModel
                {
                    CartItemID = item.CartItemID,
                    ProductTitle = item.ProductTitle,
                    OrderTitle = item.OrderTitle,
                    FinalPrice = item.CalculatedPrice,
                    SelectedFeaturesValue = item.FeaturesCombination.Split('-').ToList(),
                    ProductFeatures = featureTitles,
                    Files = item.CartItemFiles.Select(f => new FileViewModel
                    {
                        Id = f.CartItemFileId,
                        FileName = f.FileName,
                        OriginalFileName = f.OriginalFileName
                    }).ToList(),
                    Services = item.CartItemServices.Select(s => s.ServiceTitle).ToList()
                });
                cartDetails.TotalPrice += item.CalculatedPrice;
            }

            return cartDetails;
        }
        public OrderFile GetOrderFileWithFileName(int fileId)
        {
            return _context.OrderFiles.Include(f => f.Detail)
                .ThenInclude(o => o.Order)
                .FirstOrDefault(o => o.FileId == fileId);
        }

        public bool DeleteCartItem(int cartItemId)
        {
            var cartItem = _context.CartItems
                .Include(c => c.CartItemServices)
                .Include(c => c.CartItemFiles)
                .FirstOrDefault(c => c.CartItemID == cartItemId);

            if (cartItem == null) return false;

            foreach (var file in cartItem.CartItemFiles)
            {
                _fileStorage.DeleteTempFile(file.CartItemID, file.FileName);
                _context.CartItemFiles.Remove(file);
            }
            _fileStorage.CleanupEmptyTempFolder(cartItem.CartItemID);

            _context.CartItemServices.RemoveRange(cartItem.CartItemServices);
            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();
            return true;
        }
        public Order ConvertCartToOrder(int cartId, PaymentResultDto payment)
        {
            if (!payment.IsSuccessful)
                throw new InvalidOperationException("پرداخت ناموفق بوده، سفارشی ساخته نمی‌شود.");

            var cart = _context.Carts
                .Include(c => c.CartItems).ThenInclude(ci => ci.CartItemServices)
                .Include(c => c.CartItems).ThenInclude(ci => ci.CartItemFiles)
                .FirstOrDefault(c => c.CartID == cartId);

            if (cart == null || !cart.CartItems.Any())
                throw new InvalidOperationException("سبد خرید خالی است.");

            var order = new Order
            {
                UserId = cart.UserID,
                CheckoutToken = cart.CartToken,
                PaymentStatus = OrderPaymentStatus.Paid,
                OrderStatusId = _orderService.GetInitialStatusId(),
                CreateDate = DateTime.Now,
                PaymentAuthority = payment.Authority,
                PaymentRefId = payment.RefId,
                PaymentDate = DateTime.Now,
                TotalPrice = cart.CartItems.Sum(i => i.CalculatedPrice),
                DiscountAmount = 0,
                OrderDetails = new List<OrderDetail>()
            };
            order.FinalPrice = order.TotalPrice - order.DiscountAmount;

            // برای فاز دوم (جابجایی فیزیکی فایل بعد از SaveChanges) نگه می‌داریم کدوم فایل از کدوم CartItem اومده
            var pendingFileMoves = new List<(int cartItemId, string tempFileName, OrderFile orderFile)>();

            foreach (var item in cart.CartItems)
            {
                var detail = new OrderDetail
                {
                    ProductId = item.ProductID,
                    ProductTitle = item.ProductTitle,
                    OrderDetailTitle = item.OrderTitle,
                    FeaturesCombination = item.FeaturesCombination,
                    Services = string.Join(",", item.CartItemServices.Select(s => s.ServiceTitle)),
                    Price = item.CalculatedPrice,
                    OrderDetailServices = item.CartItemServices.Select(s => new OrderDetailService
                    {
                        ServiceID = s.ServiceID,
                        ServiceTitle = s.ServiceTitle,
                        ServicePrice = s.ServicePrice
                    }).ToList(),
                    Files = new List<OrderFile>()
                };

                foreach (var file in item.CartItemFiles)
                {
                    var orderFile = new OrderFile
                    {
                        FileName = file.FileName,
                        OriginalFileName = file.OriginalFileName,
                        IsTemp = false,
                        UploadDate = file.UploadDate
                    };
                    detail.Files.Add(orderFile);
                    pendingFileMoves.Add((item.CartItemID, file.FileName, orderFile));
                }

                order.OrderDetails.Add(detail);
            }

            _context.Orders.Add(order);
            _context.SaveChanges(); // اینجا OrderId, DetailId, FileId همه واقعی می‌شن

            // فاز دوم: حالا که FileId واقعیه، فایل فیزیکی رو جابجا کن
            foreach (var (cartItemId, tempFileName, orderFile) in pendingFileMoves)
            {
                MoveFileFromTempToPermanent(cartItemId, tempFileName, orderFile.FileId);
            }

            // پاکسازی سبد
            var cartItemIds = cart.CartItems.Select(i => i.CartItemID).ToList();
            _context.CartItemServices.RemoveRange(cart.CartItems.SelectMany(i => i.CartItemServices));
            _context.CartItemFiles.RemoveRange(cart.CartItems.SelectMany(i => i.CartItemFiles));
            _context.CartItems.RemoveRange(cart.CartItems);
            _context.SaveChanges();

            foreach (var cartItemId in cartItemIds)
                CleanupCartTempFolder(cartItemId);

            return order;
        }
        //روند کار رو باید عوض کرد
        //زمانی که یه محصول به سبد خرید اضافه میشه نباید به سفارشات اضافه بشه
        // و بعد از پرداخت به یک سفارش تبدیل بشه
        //باید برای هندل کردن سبد خرید یک مدل برای فایل ها اضافه بشه
        #endregion
        #region Files
        public CartItemFile GetCartItemFile(int cartItemFileId)
        {
            return _context.CartItemFiles
                .Include(f => f.CartItem)
                .ThenInclude(ci => ci.Cart)
                .FirstOrDefault(f => f.CartItemFileId == cartItemFileId);
        }
        private void SaveCartItemFiles(int cartItemId, List<IFormFile> files)
        {
            if (files == null || !files.Any())
                return;

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Temp", $"Cart_{cartItemId}");
            Directory.CreateDirectory(folderPath);

            foreach (var file in files)
            {
                string uniqueFileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
                string fullPath = Path.Combine(folderPath, uniqueFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                _context.CartItemFiles.Add(new CartItemFile
                {
                    CartItemID = cartItemId,
                    FileName = uniqueFileName,
                    OriginalFileName = file.FileName,
                    UploadDate = DateTime.Now
                });
            }

            _context.SaveChanges();
        }

        private void MoveFileFromTempToPermanent(int cartItemId, string tempFileName, int orderFileId)
        {
            string sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Temp",
                $"Cart_{cartItemId}", tempFileName);

            string destFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Files",
                $"File_{orderFileId}");
            Directory.CreateDirectory(destFolder);

            string destPath = Path.Combine(destFolder, tempFileName);

            if (File.Exists(sourcePath))
                File.Move(sourcePath, destPath, overwrite: true);
        }

        private void CleanupCartTempFolder(int cartItemId)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Temp", $"Cart_{cartItemId}");
            if (Directory.Exists(folderPath) && !Directory.EnumerateFileSystemEntries(folderPath).Any())
                Directory.Delete(folderPath);
        }


        #endregion
        #region Before Refactor
        //public Cart CreateCartOrAddItem(int productId, string featuresCombination, List<int> serviceIds, string userName = null, string token = null, Cart cart = null)
        //{

        //    if (cart == null)
        //    {
        //        cart = new Cart();
        //        DateTime createDate = DateTime.Now;
        //        if (userName != null)
        //        {
        //            int userId = _userService.GetUserIdWithUserName(userName);
        //            cart.UserID = userId;
        //            cart.CreateDate = createDate;
        //        }
        //        if (token != null)
        //        {
        //            cart.CartToken = token;
        //            cart.CreateDate = DateTime.Now;
        //        }
        //    }


        //    string productName = _productService.GetProductTitleById(productId);
        //    long calculatedPrice = (long)_productService.CalculatePrice(productId, featuresCombination, serviceIds);

        //    var cartItemServices = CreateCartItemServices(serviceIds);

        //    cart.CartItems = new List<CartItem>();
        //    cart.CartItems.Add(new CartItem
        //    {
        //        ProductID = productId,
        //        ProductTitle = productName,
        //        FeaturesCombination = featuresCombination,
        //        CalculatedPrice = calculatedPrice,
        //        CartItemServices = cartItemServices
        //    });
        //    if (cart.CartID != 0)
        //        _context.Carts.Update(cart);
        //    else
        //        _context.Carts.Add(cart);

        //    _context.SaveChanges();
        //    return cart;
        //}

        //    public CartDetailsViewModel ShowCart(string userName = null, string token = null)
        //    {
        //        var cart = GetUserActiveCart(userName, token);
        //        CartDetailsViewModel cartDetails = new();
        //        cartDetails.Cart = cart;

        //        if (userName != null)
        //        {
        //            List<AddressForCartViewModel> addresses = _userService.GetUserAdresses(userName).Select(i => new AddressForCartViewModel()
        //            {
        //                UserAddressId = i.AddressId,
        //                FullAddress = i.FullAddress + "، پلاک" + i.HouseNumber,
        //                AddressTitle = i.AddressTitle
        //            }).ToList();
        //            cartDetails.UserAddresses = addresses;
        //        }

        //        List<CartItemViewModel> cartItemViewModels = new();
        //        var userCartItems = GetUserCartItems(cart.CartID);
        //        int OrderId = _context.Orders
        //            .FirstOrDefault(o => (o.UserId == cart.UserID ||
        //            o.CheckoutToken == cart.CartToken) &&
        //            o.PaymentStatus == DataLayer.Enums.OrderPaymentStatus.Cart).OrderId;

        //        var orderDetail = _context.OrderDetails.Include(o => o.Files).Where(o => o.OrderId == OrderId).ToList();

        //        for (int i = 0; i < userCartItems.Count; i++)
        //        {
        //            int parentProductId = (int)_productService.GetProductById(userCartItems[i].ProductID).ParentId;
        //            //اینجا باید اون ویژگی هایی رو بگیرم که مقدار هم دارن
        //            //var featuresTitle = _context.ProductFeatures
        //            //    .Where(p => p.ProductId == parentProductId)
        //            //    .Select(x => x.Feature.FeatureTitle)
        //            //    .ToList();

        //            var featureTitles = _context.ProductFeatureValues
        //.Where(pfv => pfv.ProductId == userCartItems[i].ProductID)
        //.Select(pfv => pfv.Feature.FeatureTitle)
        //.Distinct()
        //.ToList();

        //            List<string> orderServices = _productService.GetOrderDetailsServices(orderDetail[i].DetailId).Select(o => o.ServiceTitle).ToList();

        //            CartItemViewModel cartItem = new()
        //            {
        //                CartItemID = userCartItems[i].CartItemID,
        //                OrderDetailID = orderDetail[i].DetailId,
        //                ProductTitle = userCartItems[i].ProductTitle,
        //                FinalPrice = userCartItems[i].CalculatedPrice,
        //                SelectedFeaturesValue = userCartItems[i].FeaturesCombination.Split('-').ToList(),
        //                ProductFeatures = featureTitles,
        //                OrderTitle = orderDetail[i].OrderDetailTitle,

        //                Files = orderDetail[i].Files.Select(f => new FileViewModel()
        //                {
        //                    Id = f.FileId,
        //                    FileName = f.FileName,
        //                    OriginalFileName = f.OriginalFileName
        //                }).ToList(),
        //                Services = orderServices
        //            };
        //            //cartItemViewModels.Add(cartItem);
        //            cartDetails.Items.Add(cartItem);
        //            cartDetails.TotalPrice += userCartItems[i].CalculatedPrice;
        //        }


        //        return cartDetails;
        //    }


        //public bool DeleteCartItem(int cartItemId, int orderDetailId)
        //{
        //    var cartItem = _context.CartItems.Find(cartItemId);
        //    if (cartItem == null)
        //        return false;
        //    var cart = _context.Carts.Find(cartItem.CartID);

        //    List<CartItemService> services = _context.CartItemServices
        //        .Where(s => s.CartItemID == cartItemId)
        //        .ToList();

        //    foreach (var service in services)
        //        _context.CartItemServices.Remove(service);

        //    var orderDetail = _context.OrderDetails.Find(orderDetailId);
        //    var order = _context.Orders.Find(orderDetail.OrderId);

        //    order.FinalPrice -= cartItem.CalculatedPrice;
        //    order.TotalPrice -= cartItem.CalculatedPrice;

        //    _context.Orders.Update(order);

        //    List<OrderDetailService> orderServices = _context.OrderDetailServices
        //        .Where(os => os.OrderDetailID == orderDetailId)
        //        .ToList();

        //    foreach (var service in orderServices)
        //        _context.OrderDetailServices.Remove(service);

        //    List<OrderFile> files = _context.OrderFiles
        //        .Where(f=> f.DetailId == orderDetailId)
        //        .ToList();

        //    foreach(var file in files)
        //    {
        //        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Temp", "Temp_" + file.FileId, file.FileName);

        //        if (File.Exists(imagePath))
        //        {
        //            File.Delete(imagePath);
        //        }
        //        _context.OrderFiles.Remove(file);
        //    }
        //    _context.SaveChanges();
        //    return true;
        //}
        #endregion
    }
}
