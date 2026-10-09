using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.DTOs.Products
{
    public class HomeCarouselViewModel
    {
        public List<ShowProductListViewModel> BestSellers { get; set; } = new();
        public List<HomeProductSectionViewModel> ProductSections { get; set; } = new ();

    }
    public class ShowProductListViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageName { get; set; }
        public string GroupName { get; set; }
    }
    public class HomeProductSectionViewModel
    {
        public string Title { get; set; } = string.Empty;

        public List<ShowProductListViewModel> Products { get; set; } = new();
    }
    public class ProductPriceViewModel
    {
        public int ProductPriceId { get; set; }
        public decimal Price { get; set; }
    }
}
