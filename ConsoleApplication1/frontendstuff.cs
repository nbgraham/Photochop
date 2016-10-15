using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Hello! My name is Jordan, and I don't know how to program!
namespace ConsoleApplication1
{
    class frontendstuff
{
        private List<string> listOfFiles;
        private string session_id;
        // The four corners of the rectangle, x1 y1 and x2 y2
        private int x1;
        private int x2;
        private int y1;
        private int y2;
    
        frontendstuff()
        {
            session_id = "";
            x1 = 0;
            x2 = 0;
            y1 = 0;
            y2 = 0;
        }

        frontendstuff(string sid, int x1, int y1, int x2, int y2)
        {
            session_id = sid;
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }

        void setRectangle(int x1, int y1, int x2, int y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }

        void addtoFileList(string filename)
        {
            listOfFiles.Add(filename);
        }

        void setSessionID(string id)
        {
            session_id = id;
        }

        int[][] getRectangle()
        {
            int[] coord1 = { x1, y1 };
            int[] coord2 = { x2, y2 };
            int[][] coords = { coord1, coord2 };
            return coords;
        }

        string getSessionID()
        {
            return session_id;
        }

        List<string> getFileList()
        {
            return listOfFiles;
        }
        
    }
}
