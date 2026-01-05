using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.Convertors
{
    public class ImageConvertor
    {
        public void ResizeImage(string inputImagePath, string outputImagePath, int newWidth)
        {
            const int quality = 95;

            try
            {
                using (var image = Image.Load(inputImagePath))
                {
                    int newHeight = (int)((double)newWidth / image.Width * image.Height);

                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(newWidth, newHeight),
                        Mode = ResizeMode.Max,
                        Sampler = KnownResamplers.Lanczos3
                    }));

                    image.Metadata.ExifProfile = null;

                    var encoder = new SixLabors.ImageSharp.Formats.Webp.WebpEncoder
                    {
                        Quality = 90,
                        FileFormat = SixLabors.ImageSharp.Formats.Webp.WebpFileFormatType.Lossy
                    };

                    image.Save(outputImagePath, encoder);
                }

            }
            catch (Exception ex)
            {
                throw new InvalidCastException("Failed to resize and save the image", ex);
            }
        }
    }
}
