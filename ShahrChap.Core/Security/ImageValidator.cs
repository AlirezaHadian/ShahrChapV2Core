using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SixLabors.ImageSharp;

namespace ShahrChap.Core.Security
{
    public static class ImageValidator
    {
        public static bool IsImage(this IFormFile file)
        {
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var image = Image.Load(stream);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
