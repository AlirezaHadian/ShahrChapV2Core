using Microsoft.AspNetCore.Mvc.Rendering;
using ShahrChap.DataLayer.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShahrChap.Core.DTOs.Products;

namespace ShahrChap.Core.Services.Interfaces
{
    public interface IProductService
    {
        #region Group
        List<ProductGroup> GetAllGroups();
        List<SelectListItem> GetGroupForManageProducts(); 
        List<SelectListItem> GetSubGroupForManageProducts(int groupId);
        #endregion
        #region Product
        List<ShowProductForAdminViewModel> GetProductsForAdmin();
        int AddProudct(Product product, IFormFile imgProduct);
        Product GetProductById(int productId);
        void UpdateProduct(Product product, IFormFile imgProduct);
        string AddProductImage(IFormFile productImage);
        void DeleteProductImage(string currentProductName);
        #endregion

    }
}
