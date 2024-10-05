using ShahrChap.DataLayer.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.Services.Interfaces
{
    public interface IProductService
    {
        #region Group
        List<ProductGroup> GetAllGroups();
        #endregion
    }
}
