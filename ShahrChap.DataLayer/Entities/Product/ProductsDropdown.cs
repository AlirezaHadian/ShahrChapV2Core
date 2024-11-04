using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.DataLayer.Entities.Product
{
    public class ProductsDropdown
    {
        public int PD_Id { get; set; }
        public int GroupId { get; set; }
        public int SubGroupId { get; set; }
        public string ProductTitle { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public bool IsDelete { get; set; }
    }
}
