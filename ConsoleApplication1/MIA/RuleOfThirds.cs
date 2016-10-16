using AForge.Imaging;
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
        new public Rectangle mostInterestingArea(System.Drawing.Image img)
        {
            Bitmap bitmap = new Bitmap(img);

            BlobCounter bc = new BlobCounter();

            bc.ProcessImage(bitmap);

            Rectangle[] objs = bc.GetObjectsRectangles();

            foreach (Rectangle obj in objs)
                Console.WriteLine(obj);
            
            Console.WriteLine("Blob Size: " + objs.Length);

            List<Size> _sizes = sizes(img);

            List<Rectangle> _areas = areas(img, _sizes);

            return bestRuleOfThirdsBound(objs, _areas);
        }

        private Rectangle bestRuleOfThirdsBound(
           Rectangle[] objs, List<Rectangle> bounds)
        {
            int maxIntersections = 0;
            Rectangle rect = default(Rectangle);

            Console.WriteLine("Intersections: ");

            foreach (Rectangle bound in bounds)
            {
                int intersections = Intersections(bound, objs);

                Console.WriteLine(intersections);

                if(intersections > maxIntersections)
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

            for (int r = 0; r < 3; r++)
                for (int c = 1; c < 3; c++)
                {
                    int x = c * bound.Width / 3;
                    int y1 = r * bound.Height / 3,
                        y2 = (r + 1) * bound.Height / 3;

                    bool intersection = false;
                    foreach (Rectangle obj in objs)
                        if (IntersectionVert(obj, x, y1, y2))
                            intersection = true;

                    if (intersection) intersections++;
                }

            for (int r = 1; r < 3; r++)
                for (int c = 0; c < 3; c++)
                {
                    int x1 = c * bound.Width / 3,
                        x2 = (c + 1) * bound.Width / 3;

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

            for (int height = 300; height < img.Height; height += 100)
                for (int width = 300; width < img.Width; width += 100)
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
                for (int y = 0; y + size.Height < img.Height; y += 50)
                    for (int x = 0; x + size.Width < img.Width; x += 50)
                    {
                        Rectangle area = new Rectangle(x, y, size.Width, size.Height);
                        //Console.WriteLine(area);
                        areas.Add(area);
                    }
            }

            //Console.WriteLine("Area Count: " + areas.Count);

            return areas;
        }
    }
}
