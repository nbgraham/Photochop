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
            Bitmap bitmap = new Bitmap(img);

            List<Size> _sizes = sizes(img);

            List<Rectangle> _areas = areas(img, _sizes);

            Size tileSize = new Size(100, 100);

            Color[,] tileAvgColors = 
                AverageColorsOfTiles(bitmap, tileSize);

            int[,] tileColorfulness =
                ColorfulnessOfTiles(bitmap, tileAvgColors, tileSize);

            return mostColorfulArea(_areas, tileAvgColors,
                                    tileColorfulness, tileSize);
        }

        public int Colorfulness(
            int[,] tileColorfulness,
            Color[,] tileAvgColors, Size tileSize,
            Rectangle bound)
        {
            int colLeft = bound.Left / tileSize.Width,
                colRight = bound.Right / tileSize.Width,
                rowTop = bound.Top / tileSize.Height,
                rowBottom = bound.Bottom / tileSize.Height;


            Color avgColor = 
                AverageColor(tileAvgColors, tileSize, bound);

            int colorfulness = 0;

            int tileCount = (colRight - colLeft) *
                            (rowBottom - rowTop);

            for (int row = rowTop; row < rowBottom; row++)
                for (int col = colLeft; col < colRight; col++)
                {
                    colorfulness += tileColorfulness[col, row] +
                                    colorDifference(
                                        avgColor, tileAvgColors[col, row]);
                }

            colorfulness /= tileCount;

            return colorfulness;
        }

        public Color AverageColor(
            Color[,] tileAvgColors, Size tileSize,
            Rectangle bound)
        {
            int colLeft = bound.Left / tileSize.Width,
                colRight = bound.Right / tileSize.Width,
                rowTop = bound.Top / tileSize.Height,
                rowBottom = bound.Bottom / tileSize.Height;

            int tileCount = (colRight - colLeft) *
                             (rowBottom - rowTop);

            int redSum = 0, greenSum = 0, blueSum = 0;

            for(int row = rowTop; row < rowBottom; row++)
                for(int col = colLeft; col < colRight; col++)
                {
                    Color color = tileAvgColors[col,row];

                    redSum += color.R;
                    greenSum += color.G;
                    blueSum += color.B;
                }

            int redAvg = redSum / tileCount,
               greenAvg = greenSum / tileCount,
               blueAvg = blueSum / tileCount;

            int argb = 0;

            argb |= redAvg << 16;
            argb |= blueAvg << 8;
            argb |= greenAvg << 0;

            Color avgColor = Color.FromArgb(argb);

            return avgColor;
        }

        public int[,] ColorfulnessOfTiles(
            Bitmap bm, 
            Color[,] tileAvgColors,
            Size tileSize)
        {
            int cols = bm.Width / tileSize.Width;
            int rows = bm.Height / tileSize.Height;

            int[,] tileColorfulNess = new int[cols, rows];

            Console.WriteLine("cols: " + cols + " rows: " + rows);

            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                {
                    int x = col * tileSize.Width,
                        y = row * tileSize.Height;

                    Rectangle bound = new Rectangle(x, y, tileSize.Width, tileSize.Height);

                    tileColorfulNess[col, row] = ColorfulnessOfTile(
                        bm, bound, tileAvgColors[col, row]);
                }

            return tileColorfulNess;
        }
        
        public int ColorfulnessOfTile(
            Bitmap bitmap,
            Rectangle bound,
            Color avgColor)
        {
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
      
        public Color[,] AverageColorsOfTiles(
            Bitmap bm, Size tileSize)
        {
            int cols = bm.Width / tileSize.Width;
            int rows = bm.Height / tileSize.Height;

            Console.WriteLine("cols: " + cols + " rows: " + rows);
            Color[,] avgColors = new Color[cols, rows];

            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                {
                    int x = col * tileSize.Width,
                        y = row * tileSize.Height;

                    Rectangle bound = new Rectangle(x, y, tileSize.Width, tileSize.Height);

                    avgColors[col, row] = averageColor(bm, bound);
                }

            return avgColors;
        }

        private List<Size> sizes(Image img)
        {
            List<Size> sizes = new List<Size>();

            for (int height = 300; height < img.Height; height += 200)
                for (int width = 300; width < img.Width; width += 200)
                {
                    Size size = new Size(width, height);

                    Console.WriteLine(size);
                    sizes.Add(size);
                }

            Console.WriteLine("Size Count: " + sizes.Count);

            return sizes;
        }

        private List<Rectangle> areas(Image img, List<Size> sizes)
        {
            List<Rectangle> areas = new List<Rectangle>();

            foreach (Size size in sizes)
            {
                for (int y = 0; y + size.Height < img.Height; y += 200)
                    for (int x = 0; x + size.Width < img.Width; x += 200)
                    {
                        Rectangle area = new Rectangle(x, y, size.Width, size.Height);
                        //Console.WriteLine(area);
                        areas.Add(area);
                    }
            }

            Console.WriteLine("Area Count: " + areas.Count);

            return areas;
        }

        private Rectangle mostColorfulArea(
            List<Rectangle> areas,
            Color[,] tileAvgColors,
            int[,] tileColorfulness,
            Size tileSize)
        {
            int maxColorfulness = 0;

            Rectangle mostColorfulArea = default(Rectangle);

            int i = 0;
            foreach (Rectangle area in areas)
            {
                Console.WriteLine("Progress: " + i + "/" + areas.Count);
                i++;

                int _colorfulness = 
                    Colorfulness(tileColorfulness, tileAvgColors, 
                                 tileSize, area);

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
                for (int x = bound.Left; x < bound.Right; x++)
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