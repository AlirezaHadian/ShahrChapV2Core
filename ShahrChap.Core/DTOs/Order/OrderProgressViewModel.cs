using ShahrChap.DataLayer.Entities.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.DTOs.Order
{
    public class OrderProgressViewModel
    {
        public int OrderId { get; set; }
        public DateTime CreateDate { get; set; }
        public string StatusTitle { get; set; }
        public string StatusColor { get; set; }
        public string StatusIcon { get; set; }
        public int Percent { get; set; }
    }
}
