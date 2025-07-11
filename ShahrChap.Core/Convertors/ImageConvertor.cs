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
            const int quality = 90;

            try
            {
                using (var image = Image.Load<Rgba32>(inputImagePath))
                {
                    int newHieght = (int)(double)newWidth / image.Width * image.Height;

                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(newWidth, newHieght),
                        Mode = ResizeMode.Max,
                        Sampler = KnownResamplers.Lanczos3
                    }));

                    image.Metadata.ExifProfile = null;

                    string outputDirectory = Path.GetDirectoryName(outputImagePath);
                    if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }

                    var encoder = new JpegEncoder
                    {
                        Quality = quality
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
