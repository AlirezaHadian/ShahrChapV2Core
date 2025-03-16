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
            string ImageName
        );
}
