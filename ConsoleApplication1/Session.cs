using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConsoleApplication1
{
    class Session
    {
        private readonly static TimeSpan MAX_SESSION_DURATION = TimeSpan.FromMinutes(10);


        private string sessionID;

        private DateTime start;

        private List<Rectangle> mostInterestingAreas;

        public Session(string sessionID,
                       Collection<MIAResource> resources)
        {
            this.sessionID = sessionID;

            start = DateTime.Now;

            mostInterestingAreas = new List<Rectangle>();

            foreach (MIAResource resource in resources)
            {
                Rectangle mostInterestingArea = 
                    resource.mostInterestingArea();

                mostInterestingAreas.Add(mostInterestingArea);
            }
        }

        public Boolean expired()
        {
            DateTime now = DateTime.Now;

            if (now - start > MAX_SESSION_DURATION)
                return true;

            return false;
        }

        public List<Rectangle> MostInterestingAreas
        {
            get;
        }

    }
}
