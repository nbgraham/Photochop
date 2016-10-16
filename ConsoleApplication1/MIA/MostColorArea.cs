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
        int[,] tilesMeanRG,
               tilesMeanYB,
               tilesVarRG,
               tilesVarYB;

        Size tileSize;
        int pixelsInTile;

        public Rectangle mostInterestingArea(Image img)
        {
            Bitmap bitmap = new Bitmap(img);

            List<Size> _sizes = sizes(img);

            List<Rectangle> _areas = areas(img, _sizes);

            tileSize = new Size(50, 50);
            pixelsInTile = 50 * 50;

            setTilesMeanRG_YB(bitmap);
            setTilesVarianceRG_YB(bitmap);

            return mostColorfulArea(_areas);
        }

        private double Colorfulness(Rectangle bound)
        {
            int varRG = VarianceRG(bound);
            int varYB = VarianceYB(bound);

            int meanRG = MeanRG(bound),
                meanYB = MeanYB(bound);

            double stdRGYB = Math.Sqrt(varRG + varYB);
            double meanRGYB = Math.Sqrt(meanRG * meanRG + meanYB * meanYB);

            return stdRGYB + 0.3 * meanRGYB;
        }
        
        private int VarianceRG(Rectangle bound)
        {
            int startCol = bound.Left / tileSize.Width,
              endCol = bound.Right / tileSize.Width,
              startRow = bound.Top / tileSize.Height,
              endRow = bound.Bottom / tileSize.Height;

            int sum = 0;

            int meanRG = MeanRG(bound);

            for (int row = startRow; row < endRow; row++)
                for (int col = startCol; col < endCol; col++)
                    sum += tilesVarRG[col, row] + (meanRG - tilesMeanRG[col, row]) *
                                                  (meanRG - tilesMeanRG[col, row]);

            int count = (bound.Width / tileSize.Width) *
                        (bound.Height / tileSize.Height);

            return sum / count;
        }

        private int VarianceYB(Rectangle bound)
        {
            int startCol = bound.Left / tileSize.Width,
              endCol = bound.Right / tileSize.Width,
              startRow = bound.Top / tileSize.Height,
              endRow = bound.Bottom / tileSize.Height;

            int sum = 0;

            int meanYB = MeanYB(bound);

            for (int row = startRow; row < endRow; row++)
                for (int col = startCol; col < endCol; col++)
                    sum += tilesVarYB[col, row] + (meanYB - tilesMeanYB[col, row]) *
                                                  (meanYB - tilesMeanYB[col, row]);

            int count = (bound.Width / tileSize.Width) *
                        (bound.Height / tileSize.Height);

            return sum / count;
        }

        private void setTilesVarianceRG_YB(Bitmap bitmap)
        {
            int cols = bitmap.Width / tileSize.Width;
            int rows = bitmap.Height / tileSize.Height;

            tilesVarRG = new int[cols, rows];
            tilesVarYB = new int[cols, rows];

            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                    setVarianceRG_YB(bitmap, col, row);
        }

        private void setVarianceRG_YB(Bitmap bitmap, int col, int row)
        {
            int sumRG = 0, sumYB = 0;

            int x = col * tileSize.Width,
                y = row * tileSize.Height;

            Rectangle bound = new Rectangle(x, y, tileSize.Width, tileSize.Height);

            for (int r = bound.Top; r < bound.Bottom; r++)
                for (int c = bound.Left; c < bound.Right; c++)
                {
                    Color color = bitmap.GetPixel(c, r);
                    int rg = RG(color);
                    int yb = YB(color);

                    sumRG += (tilesMeanRG[col,row] - rg) *
                             (tilesMeanRG[col,row] - rg);

                    sumYB += (tilesMeanYB[col, row] - yb) *
                             (tilesMeanYB[col, row] - yb);
                }

            tilesVarRG[col, row] = sumRG / pixelsInTile;
            tilesVarYB[col, row] = sumYB / pixelsInTile;
        }

        private int MeanRG(Rectangle bound)
        {
            int startCol = bound.Left / tileSize.Width,
                endCol = bound.Right / tileSize.Width,
                startRow = bound.Top / tileSize.Height,
                endRow = bound.Bottom / tileSize.Height;

            int sumMeanRG = 0;

            for (int row = startRow; row < endRow; row++)
                for (int col = startCol; col < endCol; col++)
                    sumMeanRG += tilesMeanRG[col,row];

            int count = (bound.Width / tileSize.Width) *
                        (bound.Height / tileSize.Height);

            return sumMeanRG / count;
        }

        private int MeanYB(Rectangle bound)
        {
            int startCol = bound.Left / tileSize.Width,
               endCol = bound.Right / tileSize.Width,
               startRow = bound.Top / tileSize.Height,
               endRow = bound.Bottom / tileSize.Height;

            int sumMeanYB = 0;

            for (int row = startRow; row < endRow; row++)
                for (int col = startCol; col < endCol; col++)
                    sumMeanYB += tilesMeanYB[col, row];

            int count = (bound.Width / tileSize.Width) *
                        (bound.Height / tileSize.Height);

            return sumMeanYB / count;
        }

        private void setTilesMeanRG_YB(Bitmap bitmap)
        {
            int cols = bitmap.Width / tileSize.Width;
            int rows = bitmap.Height / tileSize.Height;

            tilesMeanRG = new int[cols, rows];
            tilesMeanYB = new int[cols, rows];

            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                    setMeanRG_YB(bitmap, col, row);
        }

        private void setMeanRG_YB(Bitmap bitmap, int col, int row)
        {
            int sumRG = 0, sumYB = 0;

            int x = col * tileSize.Width,
                       y = row * tileSize.Height;

            Rectangle bound =
                new Rectangle(x, y, tileSize.Width, tileSize.Height);

            for (int j = bound.Top; j < bound.Bottom; j++)
                for(int i = bound.Left; i < bound.Right; i++)
                {
                    Color color = bitmap.GetPixel(x, y);

                    sumRG += RG(color);
                    sumYB += YB(color);
                }

            tilesMeanRG[col, row] = sumRG / pixelsInTile;
            tilesMeanYB[col, row] = sumYB / pixelsInTile;
        }

        private int RG(Color color)
        {
            return Math.Abs(color.R - color.G);
        }

        private int YB(Color color)
        {
            return Math.Abs((color.R + color.G) / 2 - color.B);
        }

        private List<Size> sizes(Image img)
        {
            List<Size> sizes = new List<Size>();

            for (int height = 300; height < img.Height; height += 100)
                for (int width = 300; width < img.Width; width += 100)
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

        private Rectangle mostColorfulArea(List<Rectangle> areas)
        {
            double maxColorfulness = 0;

            Rectangle mostColorfulArea = default(Rectangle);

            int i = 0;
            foreach (Rectangle area in areas)
            {
                Console.WriteLine("Progress: " + i + "/" + areas.Count);
                i++;

                double colorfulness = Colorfulness(area);

                if (maxColorfulness <= colorfulness)
                {
                    maxColorfulness = colorfulness;
                    mostColorfulArea = area;
                }
            }

            return mostColorfulArea;
        }

       
    }
}