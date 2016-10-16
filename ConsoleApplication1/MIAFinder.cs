using ImageProcessor;
using ImageProcessor.Imaging.Filters.EdgeDetection;
using ImageProcessor.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    // this is the building block of our 2 algorithms
  
    class MIAFinder
    {
       public Rectangle mostInterestingArea(Image img)
        {
            byte[] photoBytes = imageToByteArray(img);

            // Format is automatically detected though can be changed.
            ISupportedImageFormat format = new JpegFormat { Quality = 70 };
            Size size = new Size(150, 0);

            Size resultingSize = new Size();

            using (MemoryStream inStream = new MemoryStream(photoBytes))
                        {
                            using (MemoryStream outStream = new MemoryStream())
                            {
                                // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                                {
                                    // Load, resize, set the format and quality and save an image.
                                    imageFactory.Load(inStream)
                                                .Format(format)
                                                .EntropyCrop();

                                    ImageFactory imgFactory = ObjectCopier.Clone(imageFactory);

                                    imgFactory.DetectEdges(new Laplacian3X3EdgeFilter())
                                                .EntropyCrop();

                                    //Need to crop, not resize
                                    imageFactory.Resize(imgFactory.Image.Size);
                                              
                                    imageFactory.Save(outStream);

                                    resultingSize = imageFactory.Image.Size;
                                }
                                // Do something with the stream.

                            }
                        }

            return new Rectangle(new Point(), resultingSize);
        }

        private byte[] imageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }

        private Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}
