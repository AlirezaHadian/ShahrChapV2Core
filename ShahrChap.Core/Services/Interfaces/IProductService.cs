﻿using Microsoft.AspNetCore.Mvc.Rendering;
using ShahrChap.DataLayer.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShahrChap.Core.DTOs.Products;
using System.ComponentModel;
using Microsoft.AspNetCore.Razor.Language;
using ShahrChap.DataLayer.Entities.Product.Form;
using ShahrChap.Core.Enums;

namespace ShahrChap.Core.Services.Interfaces
{
    public interface IProductService
    {
        #region Group
        List<ProductGroup> GetAllGroups();
        List<SelectListItem> GetGroupForManageProducts();
        List<SelectListItem> GetSubGroupForManageProducts(int groupId);
        #endregion
        #region Type
        //This type will specified the type of product for the forms
        List<SelectListItem> GetTypes();
        int GetTypeFormsCount(int typeId);
        #endregion
        #region Product
        List<ShowProductForAdminViewModel> GetProductsForAdmin();
        int AddProudct(Product product, IFormFile imgProduct);
        Product GetProductById(int productId);
        void UpdateProduct(Product product, IFormFile imgProduct);
        void DeleteProduct(Product product);
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
        List<int> ProductFeatureIds(int productId);
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
        #region ProductGallery
        List<ProductGallery> GetProductGalleryListById(int productId);
        int AddImageToProduct(ProductGallery gallery, IFormFile imgGallery);
        ProductGallery GetGalleryById(int galleryId);
        void UpdateGallery(ProductGallery gallery, IFormFile imgGallery);
        void DeleteGallery(ProductGallery gallery);
        string AddImageToProductGallery(IFormFile imageGallery);
        void DeleteGalleryImage(string currentGalleryName);
        #endregion
        #region SubProduct
        List<ShowProductForAdminViewModel> GetSubProductForAdmin(int id);
        #endregion
        #region FeatureValues
        List<FeatureValue> GetAllFeatureValues(int productId);
        List<int> SubProductFeatureValueIds(int productId);
        void AddFeatureValuesToProduct(int productId, List<int> values);
        void UpdateFeatureValuesProduct(int productId, List<int> featureValues);
        //List<FeatureValue> GetFeaturesByFeatureValues(List<int> values);
        #endregion
        #region Pricing
        List<string> GetFeatureCombinations(int productId);
        void AddProductPrices(int productId, List<ProductPrice> price);
        void UpdateProductPrices(int productId, List<ProductPrice> price);
        void DeleteProductPrices(int productId);
        List<ProductPrice> GetProductPrices(int productId);
        void AddServicePrices(List<ServicePrice> servicePrices);
        bool AreCombinationsChanged(int productId, List<string> Combintations);
        List<ServicePrice> GetServicePricesForProduct(int productId);
        #endregion
        #region Forms
        List<ProductForm> GetProductForms(int productId);
        //This method is for determining the status of the product's created forms and handling it. 
        FormCreationState GetPendingForms(int productId);
        ProductForm GetProductFormById(int formId);
        #endregion
    }
}
