using AForge.Imaging;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class RuleOfThirds : MIAFinder
    {
        new public Rectangle mostInterestingArea(System.Drawing.Image img_orig)
        {
            // http://stackoverflow.com/questions/2016406/converting-bitmap-pixelformats-in-c-sharp
            Bitmap img_middle = new Bitmap(img_orig);
            Bitmap img = img_middle.Clone(new Rectangle(0, 0, img_middle.Width, img_middle.Height), System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            img_middle.Dispose();
            Bitmap bitmap = PreProcess((Bitmap)img);
            
            BlobCounter bc = new BlobCounter();

            bc.ProcessImage(bitmap);

            Rectangle[] objs = filteredObjects(bc.GetObjectsRectangles(), img);

            foreach (Rectangle obj in objs)
                Console.WriteLine(obj);
            
            Console.WriteLine("Blob Size: " + objs.Length);

            List<Size> _sizes = sizes(img);

            List<Rectangle> _areas = areas(img, _sizes);

            var ret = bestRuleOfThirdsBound(objs, _areas, img);
            img.Dispose();
            return ret;
        }

        private Rectangle[] filteredObjects(Rectangle[] objs, 
            System.Drawing.Image img)
        {
            List<Rectangle> filteredObjs = new List<Rectangle>();

            int minArea = (img.Width / 5) * (img.Height / 5);

            foreach (Rectangle obj in objs)
                if (obj.Width * obj.Height >= minArea)
                    filteredObjs.Add(obj);

            return filteredObjs.ToArray();

        }

        // source
        // http://stackoverflow.com/questions/35980717/why-doesnt-aforge-net-blobcounter-getobjectsinformation-detect-objects
        private Bitmap PreProcess(Bitmap bmp)
        {
            //Those are AForge filters "using Aforge.Imaging.Filters;"
            Grayscale gfilter = new Grayscale(0.2125, 0.7154, 0.0721);
            Invert ifilter = new Invert();
            BradleyLocalThresholding thfilter = new BradleyLocalThresholding();
            bmp = gfilter.Apply(bmp);
            thfilter.ApplyInPlace(bmp);
            ifilter.ApplyInPlace(bmp);
            return bmp;
        }
        private Rectangle bestRuleOfThirdsBound(
           Rectangle[] objs, List<Rectangle> bounds,
           System.Drawing.Image img)
        {
            int maxIntersections = 0;
            Rectangle rect = default(Rectangle);
            

            //Console.WriteLine("Intersections: ");

            Console.WriteLine(bounds.Count);

            foreach (Rectangle bound in bounds)
            {
                int intersections = Intersections(bound, objs);

                //Console.WriteLine(intersections);
                bool replace = false;

                if (intersections > maxIntersections)
                    replace = true;
                else if(intersections == maxIntersections)
                {
                    int rectArea = rect.Width * rect.Height,
                        boundArea = bound.Width * bound.Height;

                    Console.WriteLine(rectArea + " " + boundArea);
                    if (rectArea == 0 || rectArea > boundArea)
                        replace = true;
                    else if(rectArea == boundArea)
                    {
                        int rectDist = distanceSqrFromImageCenter(rect, img),
                            boundDist = distanceSqrFromImageCenter(bound, img);

                        if (rectDist < boundDist)
                            replace = true;
                    }
                }
                 
                if(replace)
                {
                    maxIntersections = intersections;

                    rect = bound;
                }
            }

            return rect;
        }

        // the amount of lines that the blobs intersect
        private int Intersections(
            Rectangle bound, Rectangle[] objs)
        {
            int intersections = 0;

            for (int r = 0; r < 30; r++)
                for (int c = 1; c < 3; c++)
                {
                    int x = c * bound.Width / 3;
                    int y1 = r * bound.Height / 30,
                        y2 = (r + 1) * bound.Height / 30;

                    bool intersection = false;
                    foreach (Rectangle obj in objs)
                        if (IntersectionVert(obj, x, y1, y2))
                            intersection = true;

                    if (intersection) intersections++;
                }

            for (int r = 1; r < 30; r++)
                for (int c = 0; c < 30; c++)
                {
                    int x1 = c * bound.Width / 30,
                        x2 = (c + 1) * bound.Width / 30;

                    int y = r * bound.Height / 3;

                    bool intersection = false;
                    foreach (Rectangle obj in objs)
                        if (IntersectionHoriz(obj, x1, x2, y))
                            intersection = true;

                    if (intersection) intersections++;
                }

            return intersections;
        }


        private bool IntersectionVert(Rectangle rect, int x, int y1, int y2)
        {
            if(rect.Left <= x && x <= rect.Right)
            {
                if ((y1 <= rect.Top && rect.Top <= y2) ||
                   (y1 <= rect.Bottom && rect.Bottom <= y2) ||
                   (rect.Top <= y1 && y1 <= rect.Bottom))
                    return true;
            }

            return false;
        }

        private Boolean IntersectionHoriz(
            Rectangle rect, int x1, int x2, int y)
        {
            if (rect.Top <= y && y <= rect.Bottom)
            {
                if ((x1 <= rect.Left && rect.Left <= x2) ||
                   (x1 <= rect.Right && rect.Right <= x2) ||
                   (rect.Left <= x1 && x1 <= rect.Right))
                    return true;
            }

            return false;
        }

        private List<Size> sizes(System.Drawing.Image img)
        {
            List<Size> sizes = new List<Size>();

            for (int height = img.Height/200 * 100; height < img.Height; height += 100)
                for (int width = img.Width/200 * 100; width < img.Width; width += 100)
                {
                    Size size = new Size(width, height);

                   // Console.WriteLine(size);
                    sizes.Add(size);
                }

          //  Console.WriteLine("Size Count: " + sizes.Count);

            return sizes;
        }

        private List<Rectangle> areas(System.Drawing.Image img, List<Size> sizes)
        {
            List<Rectangle> areas = new List<Rectangle>();

            foreach (Size size in sizes)
            {
                for (int y = 0; y + size.Height < img.Height; y += 100)
                    for (int x = 0; x + size.Width < img.Width; x += 100)
                    {
                        Rectangle area = new Rectangle(x, y, size.Width, size.Height);
                        //Console.WriteLine(area);
                        areas.Add(area);
                    }
            }

            //Console.WriteLine("Area Count: " + areas.Count);

            return areas;
        }

        private int distanceSqrFromImageCenter(Rectangle bound, System.Drawing.Image img)
        {
            Point bCenter = new Point(bound.Left + bound.Width / 2,
                                      bound.Top + bound.Height / 2),
                  imgCenter = new Point(img.Width/2, img.Height/2);

            return (bCenter.X - imgCenter.X) * (bCenter.X - imgCenter.X) +
                   (bCenter.Y - imgCenter.Y) * (bCenter.Y - imgCenter.Y);
        }
    }
}
