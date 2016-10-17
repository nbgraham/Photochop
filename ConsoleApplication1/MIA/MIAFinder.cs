using AForge.Imaging;
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
        public Rectangle mostInterestingArea(System.Drawing.Image img)
        {
            Bitmap bitmap = new Bitmap(img);

            BlobCounter bc = new BlobCounter();

            bc.ProcessImage(bitmap);

            Rectangle[] objs = bc.GetObjectsRectangles();

            Point center = getAverageRectangleCenter(objs);

            Rectangle rect = getBoundContainingSDCenters(1, objs, center);

            return rect;
        }

        private Point getAverageRectangleCenter(Rectangle[] rects)
        {
            int sumx = 0;
            int sumy = 0;
            int count = 0;

            foreach (Rectangle rect in rects)
            {
                sumx += rect.X + rect.Width / 2;
                sumy += rect.Y + rect.Height / 2;
                count++;
            }

            return new Point(sumx / count, sumy / count);
        }

        private Rectangle getBoundContainingSDCenters(double sds, Rectangle[] rects, Point averageCenter)
        {
            double sumx = 0;
            double sumy = 0;
            int count = 0;

            foreach (Rectangle rect in rects)
            {
                sumx += Math.Pow((rect.X + rect.Width / 2) - averageCenter.X, 2);
                sumy += Math.Pow((rect.Y + rect.Height / 2) - averageCenter.Y, 2);
                count++;
            }

            double xDiff = sumx/(count - 1) * sds;
            double yDiff = sumy/(count-1) * sds;

            return new Rectangle(averageCenter, new Size((int)(xDiff * 2), (int)(yDiff * 2));
        }
    }
}
