using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ServiceModel.Web;

namespace ConsoleApplication1
{
    class MyService : IMyService
    {
        static int visits = 0;
        public Stream sayHello()
        {
            visits++;
            WebOperationContext.Current.OutgoingResponse.ContentType = "text";
            return new MemoryStream(Encoding.ASCII.GetBytes("Hello, World! You are visitor #"+visits));
        }

        public Stream serveFile(string path)
        {
            string conType;
            switch (path.Substring(path.LastIndexOf('.')+1))
            {
                case "html":
                    conType = "text/html";
                    break;
                case "css":
                    conType = "text/css";
                    break;
                case "png":
                    conType = "image/png";
                    break;
                default:
                    conType = "application/octet-stream";
                    Console.WriteLine("Unsure how to serve: " + path);
                    break;
            }
            var response = WebOperationContext.Current.OutgoingResponse;
            response.ContentType = conType;
            try
            {
                return new FileStream("servable/" + path, FileMode.Open);
            }
            catch (DirectoryNotFoundException)
            {
                return do404(response);
            }
            catch (FileNotFoundException)
            {
                return do404(response);
            }
        }

        static int files = 0;
        public void fileUpload(Stream body)
        {
            files++;
            var f = File.Create("File" + files);
            body.CopyTo(f);
            f.Dispose();
        }

        public Stream fileGet(string number)
        {
            return new FileStream("File" + number, FileMode.Open);
        }

        Stream do404(OutgoingWebResponseContext response)
        {
            response.StatusCode = System.Net.HttpStatusCode.NotFound;
            response.ContentType = "text/html";
            return new FileStream("servable/errors/404.html", FileMode.Open);
        }
    }
}
