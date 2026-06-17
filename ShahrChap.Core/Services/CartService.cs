using Microsoft.EntityFrameworkCore;
using ShahrChap.Core.DTOs;
using ShahrChap.Core.DTOs.Cart;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Context;
using ShahrChap.DataLayer.Entities.Address;
using ShahrChap.DataLayer.Entities.Cart;
using ShahrChap.DataLayer.Entities.Order;
using ShahrChap.DataLayer.Entities.User;
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
        public CartService(ShahrChapContext context, IUserService userService, IProductService productService)
        {
            _context = context;
            _userService = userService;
            _productService = productService;
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
        public Cart CreateCartOrAddItem(int productId, string featuresCombination, List<int> serviceIds, string userName = null, string token = null, Cart cart = null)
        {

            if (cart == null)
            {
                cart = new Cart();
                DateTime createDate = DateTime.Now;
                if (userName != null)
                {
                    int userId = _userService.GetUserIdWithUserName(userName);
                    cart.UserID = userId;
                    cart.CreateDate = createDate;
                }
                if (token != null)
                {
                    cart.CartToken = token;
                    cart.CreateDate = DateTime.Now;
                }
            }


            string productName = _productService.GetProductTitleById(productId);
            long calculatedPrice = (long)_productService.CalculatePrice(productId, featuresCombination, serviceIds);

            var cartItemServices = CreateCartItemServices(serviceIds);

            cart.CartItems = new List<CartItem>();
            cart.CartItems.Add(new CartItem
            {
                ProductID = productId,
                ProductTitle = productName,
                FeaturesCombination = featuresCombination,
                CalculatedPrice = calculatedPrice,
                CartItemServices = cartItemServices
            });
            if (cart.CartID != 0)
                _context.Carts.Update(cart);
            else
                _context.Carts.Add(cart);

            _context.SaveChanges();
            return cart;
        }
        private List<CartItemService> CreateCartItemServices(List<int>? serviceIds)
        {
            if (serviceIds == null || !serviceIds.Any())
                return new List<CartItemService>();

            return serviceIds.Select(serviceId => new CartItemService
            {
                ServiceID = serviceId
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
            CartDetailsViewModel cartDetails = new();
            cartDetails.Cart = cart;

            if (userName != null)
            {
                List<AddressForCartViewModel> addresses = _userService.GetUserAdresses(userName).Select(i => new AddressForCartViewModel()
                {
                    UserAddressId = i.AddressId,
                    FullAddress = i.FullAddress + "، پلاک" + i.HouseNumber,
                    AddressTitle = i.AddressTitle
                }).ToList();
                cartDetails.UserAddresses = addresses;
            }

            List<CartItemViewModel> cartItemViewModels = new();
            var userCartItems = GetUserCartItems(cart.CartID);
            int OrderId = _context.Orders
                .FirstOrDefault(o => (o.UserId == cart.UserID ||
                o.CheckoutToken == cart.CartToken) &&
                o.PaymentStatus == DataLayer.Enums.OrderPaymentStatus.Cart).OrderId;

            var orderDetail = _context.OrderDetails.Include(o => o.Files).Where(o => o.OrderId == OrderId).ToList();

            for (int i = 0; i < userCartItems.Count; i++)
            {
                int parentProductId = (int)_productService.GetProductById(userCartItems[i].ProductID).ParentId;
                //اینجا باید اون ویژگی هایی رو بگیرم که مقدار هم دارن
                //var featuresTitle = _context.ProductFeatures
                //    .Where(p => p.ProductId == parentProductId)
                //    .Select(x => x.Feature.FeatureTitle)
                //    .ToList();

                var featureTitles = _context.ProductFeatureValues
    .Where(pfv => pfv.ProductId == userCartItems[i].ProductID)
    .Select(pfv => pfv.Feature.FeatureTitle)
    .Distinct()
    .ToList();

                List<string> orderServices = _productService.GetOrderDetailsServices(orderDetail[i].DetailId).Select(o => o.ServiceTitle).ToList();

                CartItemViewModel cartItem = new()
                {
                    CartItemID = userCartItems[i].CartItemID,
                    OrderDetailID = orderDetail[i].DetailId,
                    ProductTitle = userCartItems[i].ProductTitle,
                    FinalPrice = userCartItems[i].CalculatedPrice,
                    SelectedFeaturesValue = userCartItems[i].FeaturesCombination.Split('-').ToList(),
                    ProductFeatures = featureTitles,
                    OrderTitle = orderDetail[i].OrderDetailTitle,

                    Files = orderDetail[i].Files.Select(f => new FileViewModel()
                    {
                        Id = f.FileId,
                        FileName = f.FileName,
                        OriginalFileName = f.OriginalFileName
                    }).ToList(),
                    Services = orderServices
                };
                //cartItemViewModels.Add(cartItem);
                cartDetails.Items.Add(cartItem);
                cartDetails.TotalPrice += userCartItems[i].CalculatedPrice;
            }


            return cartDetails;
        }

        public OrderFile GetOrderFileWithFileName(int fileId)
        {
            return _context.OrderFiles.Include(f => f.Detail)
                .ThenInclude(o => o.Order)
                .FirstOrDefault(o => o.FileId == fileId);
        }

        public bool DeleteCartItem(int cartItemId, int orderDetailId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            if (cartItem == null)
                return false;
            var cart = _context.Carts.Find(cartItem.CartID);

            List<CartItemService> services = _context.CartItemServices
                .Where(s => s.CartItemID == cartItemId)
                .ToList();

            foreach (var service in services)
                _context.CartItemServices.Remove(service);

            var orderDetail = _context.OrderDetails.Find(orderDetailId);
            var order = _context.Orders.Find(orderDetail.OrderId);

            order.FinalPrice -= cartItem.CalculatedPrice;
            order.TotalPrice -= cartItem.CalculatedPrice;

            _context.Orders.Update(order);

            List<OrderDetailService> orderServices = _context.OrderDetailServices
                .Where(os => os.OrderDetailID == orderDetailId)
                .ToList();

            foreach (var service in orderServices)
                _context.OrderDetailServices.Remove(service);

            List<OrderFile> files = _context.OrderFiles
                .Where(f=> f.DetailId == orderDetailId)
                .ToList();

            foreach(var file in files)
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Temp", "Temp_" + file.FileId, file.FileName);

                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
                _context.OrderFiles.Remove(file);
            }
            _context.SaveChanges();
            return true;
        }
           

        //روند کار رو باید عوض کرد
        //زمانی که یه محصول به سبد خرید اضافه میشه نباید به سفارشات اضافه بشه
        // و بعد از پرداخت به یک سفارش تبدیل بشه
        //باید برای هندل کردن سبد خرید یک مدل برای فایل ها اضافه بشه
        #endregion
        }
}
