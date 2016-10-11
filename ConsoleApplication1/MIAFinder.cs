using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    // this is the building block of our 2 algorithms
  
    interface MIAFinder
    {
       Rectangle mostInterestingArea(Image img);   
    }
}
