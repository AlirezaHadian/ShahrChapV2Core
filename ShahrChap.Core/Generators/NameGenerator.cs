using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.Generators
{
    public class NameGenerator
    {
        public static string GenerateUniqCode() => Guid.NewGuid().ToString().Replace("-","");
    }
}
