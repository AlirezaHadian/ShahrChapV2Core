using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.DTOs.Products
{
    public class ShowProductListViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageName { get; set; }
        public string GroupName { get; set; }
    }

    public class ProductPriceViewModel
    {
        public int ProductPriceId { get; set; }
        public decimal Price { get; set; }
    }
}
