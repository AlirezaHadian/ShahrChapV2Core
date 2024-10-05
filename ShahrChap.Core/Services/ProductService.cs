using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Context;
using ShahrChap.DataLayer.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.Services
{
    public class ProductService:IProductService
    {
        private ShahrChapContext _context;
        public ProductService(ShahrChapContext context)
        {
            _context = context;
        }

        public List<ProductGroup> GetAllGroups()
        {
            return _context.ProductGroups.ToList();
        }
    }
}
