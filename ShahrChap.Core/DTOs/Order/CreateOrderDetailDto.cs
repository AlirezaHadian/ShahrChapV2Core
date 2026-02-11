using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.DTOs.Order
{
    public class CreateOrderDetailDto
    {
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        public string OrderTitle { get; set; }
        public string FeaturesCombination { get; set; }
        public string Services { get; set; }
        public List<IFormFile> Files { get; set; }
    }
}
