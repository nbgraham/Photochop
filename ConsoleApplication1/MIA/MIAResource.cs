using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
