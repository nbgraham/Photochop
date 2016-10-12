using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class MostColorArea : MIAFinder
    {
        public Rectangle mostInterestingArea(Image img)
        {
            List<Size> _sizes = sizes(img);

            List<Rectangle> _areas = areas(img, _sizes);

            return mostColorfulArea(img, _areas);
        }

        private List<Size> sizes(Image img)
        {
            List<Size> sizes = new List<Size>();

            for (int height = 50; height < img.Height; height += 50)
                for (int width = 50; width < img.Width; width += 50)
                {
                    Size size = new Size(width, height);

                    sizes.Add(size);
                }

            return sizes;
        }

        private List<Rectangle> areas(Image img, List<Size> sizes)
        {
            List<Rectangle> areas = new List<Rectangle>();
            
            foreach (Size size in sizes)
            {
                int xOffset = img.Width / 25,
                    yOffset = img.Height / 25;

                for (int y = 0; y + size.Height < img.Height; y += yOffset)
                    for (int x = 0; x + size.Width < img.Width; x += xOffset)
                    {
                        Rectangle area = new Rectangle(x, y, size.Width, size.Height);

                        areas.Add(area);
                    }
            }

            return areas;
        }

        private Rectangle mostColorfulArea(Image img, List<Rectangle> areas)
        {
            int maxColorfulness = 0;

            Rectangle mostColorfulArea = default(Rectangle);

            Bitmap bitmap = new Bitmap(img);

            foreach (Rectangle area in areas)
            {
                int _colorfulness = colorfulness(bitmap, area);

                if (maxColorfulness < _colorfulness)
                {
                    maxColorfulness = _colorfulness;
                    mostColorfulArea = area;
                }
            }

            return mostColorfulArea;
        }

        public Color averageColor(Bitmap bitmap, Rectangle bound)
        {
            int redSum = 0, greenSum = 0, blueSum = 0;

            int pixelCount = bound.Width * bound.Height;

            for (int y = bound.Top; y < bound.Bottom; y++)
                for(int x = bound.Left; x < bound.Right; x++)
                {
                    Color color = bitmap.GetPixel(x, y);

                    redSum += color.R;
                    greenSum += color.G;
                    blueSum += color.B;
                }

            int redAvg = redSum / pixelCount,
                greenAvg = greenSum / pixelCount,
                blueAvg = blueSum / pixelCount;

            int argb = 0;

            argb |= redAvg << 16;
            argb |= blueAvg << 8;
            argb |= greenAvg << 0;

            Color avgColor = Color.FromArgb(argb);

            return avgColor;
        }

        // returns the avg color difference from the avg color
        public int colorfulness(Bitmap bitmap, Rectangle bound)
        {
            Color avgColor = averageColor(bitmap, bound);

            int sumDiff = 0,
                pixelCount = bound.Width * bound.Height;

            for (int y = bound.Top; y < bound.Bottom; y++)
                for (int x = bound.Left; x < bound.Right; x++)
                {
                    Color color = bitmap.GetPixel(x, y);

                    sumDiff += colorDifference(color, avgColor);
                }

            int colorfulness = sumDiff / pixelCount;

            return colorfulness;
        }

        // let each color's rgb represent a vector
        // returns the eucludian distance sqrd of those vectors
        public int colorDifference(Color c1, Color c2)
        {
            byte x1 = c1.R, y1 = c1.G, z1 = c1.B,
                 x2 = c2.R, y2 = c2.G, z2 = c2.B;

            // sqrt is too costly
            return (x1 - x2) * (x1 - x2) +
                   (y1 - y2) * (y1 - y2) +
                   (z1 - z2) * (z1 - z2);
        }
    }
}
