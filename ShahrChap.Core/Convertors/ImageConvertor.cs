using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
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
            const int quality = 60;

            try
            {
                using (var image = Image.Load(inputImagePath))
                {
                    // Calculate new height to maintain aspect ratio
                    int newHeight = (int)((double)newWidth / image.Width * image.Height);

                    // Resize the image
                    image.Mutate(x => x.Resize(newWidth, newHeight));

                    // Ensure the output directory exists
                    string outputDirectory = Path.GetDirectoryName(outputImagePath);
                    if (!string.IsNullOrWhiteSpace(outputDirectory) && !Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }

                    // Configure JPEG encoder settings
                    var encoder = new JpegEncoder
                    {
                        Quality = quality
                    };

                    // Save the resized image
                    image.Save(outputImagePath, encoder);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to resize and save the image.", ex);
            }
        }
    }
}
