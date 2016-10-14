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
        static Random rng = new Random(); // For sessions.
        static Dictionary<string, List<int>> sessions = new Dictionary<string, List<int>>();

        static int visits = 0;
        public Stream sayHello()
        {
            visits++;
            WebOperationContext.Current.OutgoingResponse.ContentType = "text";
            return new MemoryStream(Encoding.ASCII.GetBytes("Hello, World! You are visitor #" + visits));
        }

        public Stream serveFile(string path)
        {
            string conType;
            switch (path.Substring(path.LastIndexOf('.') + 1))
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
        int upload(Stream body)
        {
            int myFile = files++;
            var f = File.Create("File" + myFile);
            body.CopyTo(f);
            f.Dispose();
            return myFile;
        }

        public Stream initUpload(Stream body)
        {
            string session;
            do
            {
                byte[] binarySess = new byte[15];
                rng.NextBytes(binarySess);
                session = System.Convert.ToBase64String(binarySess).Replace('/', '-');
            }
            while (sessions.ContainsKey(session));
            sessions.Add(session, new List<int>(new int[] { upload(body) }));
            return new MemoryStream(Encoding.UTF8.GetBytes(session));
        }

        public void nextUpload(string session, Stream body)
        {
            if (!sessions.ContainsKey(session))
            {
                //TODO: Error handling as per design specification
                return;
            }
            sessions[session].Add(upload(body));
        }

        public Stream fileGet(string number, string session)
        {
            return new FileStream("File" + sessions[session][int.Parse(number)], FileMode.Open);
        }

        Stream do404(OutgoingWebResponseContext response)
        {
            response.StatusCode = System.Net.HttpStatusCode.NotFound;
            response.ContentType = "text/html";
            return new FileStream("servable/errors/404.html", FileMode.Open);
        }
    }
}
