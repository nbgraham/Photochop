using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing; 
using System.ServiceModel.Web;


namespace ConsoleApplication1
{
    class MyService : IMyService
    {
        static Random rng = new Random(); // For sessions.
        static Dictionary<string, List<ImgRecord>> sessions = new Dictionary<string, List<ImgRecord>>();

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
        ImgRecord upload(Stream body)
        {
            int myFile = files++;
            var f = File.Create("File" + myFile);
            
            string name = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["name"];
            if (name == null) name = "File" + myFile;
            string type = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["type"];
            if (type == null) type = "application/octet-stream";
            body.CopyTo(f);
            f.Dispose();
            findMIA("File" + myFile);
            return new ImgRecord(myFile, name, type);
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
            sessions.Add(session, new List<ImgRecord>(new ImgRecord[] { upload(body) }));
            return new MemoryStream(Encoding.UTF8.GetBytes(session));
        }

        public Stream nextUpload(string session, Stream body)
        {
            String response;
            if (!sessions.ContainsKey(session))
            {
                response = "Invalid Session";
            }
            else
            {
                sessions[session].Add(upload(body));
                response = "OK";
            }
            return new MemoryStream(Encoding.UTF8.GetBytes(response));
        }

        public Stream fileGet(string number, string session)
        {
            ImgRecord r = sessions[session][int.Parse(number)];
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Disposition", "attachment; filename=\"" + r.name + "\"");
            WebOperationContext.Current.OutgoingResponse.ContentType = r.type;
            return new FileStream("File" + r.file, FileMode.Open);
        }

        

        Stream do404(OutgoingWebResponseContext response)
        {
            response.StatusCode = System.Net.HttpStatusCode.NotFound;
            response.ContentType = "text/html";
            return new FileStream("servable/errors/404.html", FileMode.Open);
        }


        //Finding Most interesting Part 
        MIAFinder MIAfinder = new MIAFinder();
        void findMIA(String file)
        {

            //  System.Drawing.Image imageFile  =   System.Drawing.Image.FromFile(file);
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (Image original = Image.FromStream(fs))
                {
                    Rectangle rect = MIAfinder.mostInterestingArea(original);

                    Bitmap src = Image.FromFile(file) as Bitmap;
                    Bitmap target = new Bitmap(rect.Width, rect.Height);

                    using (Graphics g = Graphics.FromImage(target))
                    {
                        g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                         rect,
                                         GraphicsUnit.Pixel);
                    }


                }
            }
        //
        }
        
    }

    class ImgRecord
    {
        public int file;
        public string name, type;
        public ImgRecord(int f, string n, string t)
        {
            file = f;
            name = n;
            type = t;
        }
    }
}
