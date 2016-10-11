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
            WebHttpBinding bind = new WebHttpBinding();
            //MaxBufferSize and TransferMode pulled from http://stackoverflow.com/questions/1354749/wcf-service-to-accept-a-post-encoded-multipart-form-data
            bind.MaxBufferSize = 65536;
            bind.TransferMode = TransferMode.Streamed;
            bind.MaxReceivedMessageSize = 10 * 1024 * 1024;
            host.AddServiceEndpoint(typeof(IMyService), bind, "");
            host.Open();
            Console.WriteLine("Return to quit...");
            Console.ReadLine();
            host.Close();
        }
    }
}
