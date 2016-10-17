using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using System.IO;
using ImageProcessor;

namespace ConsoleApplication1
{
    class MIAResource
    {
        private readonly Image img;

        private readonly MIAFinder algorithm;

        public MIAResource(Image img, MIAFinder algorithm)
        {
            this.img = img;
            this.algorithm = algorithm;
        }

        public Rectangle mostInterestingArea()
        {
            return algorithm.mostInterestingArea(img);
        }

        public Image crop(Image img, Rectangle rect)
        {
            byte[] photoBytes = imageToByteArray(img);

            // Format is automatically detected though can be changed.
            ISupportedImageFormat format = new JpegFormat { Quality = 70 };

            Image resultingImage = default(Image);

            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                                    .Crop(rect);


                        resultingImage = imageFactory.Image;
                    }

                }
            }

            return resultingImage;

        }

        private byte[] imageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
    }
}
