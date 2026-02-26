using Microsoft.AspNetCore.Http;
using Microsoft.Build.Framework;
using ShahrChap.DataLayer.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.DTOs.Products
{
    public record ShowProductForAdminViewModel(
            int ProductId,
            string ProductTitle,
            string ImageName,
            bool isDesignable
        );

    public record ParentProductForShowViewModel(
        Product ParentProduct,
        List<ShowProductListViewModel> SubProducts
    );

    public record SubProductForShowViewMode(
        Product SubProduct, 
        List<ProductFeature> ProductFeatures,
        List<FeatureValue> ProductFeatureValues,
        List<Service> Services);

    public record FinalOrderViewModel(
        int ProductId,
        string OrderTitle,
        string FeaturesCombination,
        List<int> ServiceIds,
        List<IFormFile> OrderFiles);
}
