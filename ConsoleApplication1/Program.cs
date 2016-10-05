using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new WebServiceHost(typeof(MyService), new Uri("http://localhost:80/"));
            var endpoint = host.AddServiceEndpoint(typeof(IMyService), new WebHttpBinding(), "");
            host.Open();
            Console.WriteLine("Return to quit...");
            Console.ReadLine();
            host.Close();
        }
    }
}
