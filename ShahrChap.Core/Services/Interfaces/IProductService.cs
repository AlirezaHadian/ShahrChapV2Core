using Microsoft.AspNetCore.Mvc.Rendering;
using ShahrChap.DataLayer.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShahrChap.Core.DTOs.Products;
using System.ComponentModel;

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
        #region Feature
        List<Feature> GetAllFeatures();
        List<ProductFeature> GetProductFeatures(int productId);
        int CreateFeature(Feature feature);
        Feature GetFeatureById(int featureId);
        void UpdateFeature(Feature feature);
        void DeleteFeature(Feature feature);
        void AddFeaturesToProduct(int productId, List<int> features);
        void UpdateFeaturesProduct(int productId, List<int> features);
        List<int> ProductFeatures(int productId);
        List<FeatureValue> GetFeatureValues(int featureId);
        int CreateFeatureValue(FeatureValue value);
        FeatureValue GetFeatureValueById(int valueId);
        void UpdateFeatureValue(FeatureValue value);
        void DeleteFeatureValue(FeatureValue value);
        #endregion
        #region Service
        List<Service> GetAllServices();
        List<Service> GetProductServices(int productId);
        int CreateService(Service service);
        Service GetServiceById(int serviceId);
        void UpdateService(Service service);
        void DeleteService(Service service);
        #endregion
    }
}
