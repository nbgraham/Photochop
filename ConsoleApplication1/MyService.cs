using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.ServiceModel.Web;
using System.Threading;

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

            string name = getParam("name");
            if (name == null) name = "File" + myFile;
            string type = getParam("type");
            if (type == null) type = "application/octet-stream";
            body.CopyTo(f);
            f.Dispose();
           //findMIA("File" + myFile);
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
            return safeOpen("File" + r.file);
        }



        Stream do404(OutgoingWebResponseContext response)
        {
            response.StatusCode = System.Net.HttpStatusCode.NotFound;
            response.ContentType = "text/html";
            return new FileStream("servable/errors/404.html", FileMode.Open);
        }

        public Stream getMIA(string index, string session)
        {
            ImgRecord r = sessions[session][int.Parse(index)];
            Stream fstream = safeOpen("File" + r.file);
            Bitmap image = new Bitmap(fstream);
            Rectangle MIARect = new MIAFinder().mostInterestingArea(image);
            /*
            String[] parts = imageFilePath.Split('-');
            String index = parts[0];
            String session = parts[1];
            */
            
            MIARect = trimRect(MIARect, image);

            fstream.Dispose();
            image.Dispose();

            //Rule of thirds 

            Rectangle ROTRect = new Rectangle(10, 10, 30, 30);

            //Most colorful 

            Rectangle MCRect = new Rectangle(20, 10, 40, 40);

            String styleText = "" + MIARect.Top + ";" + MIARect.Left + ";" + MIARect.Width + ";" + MIARect.Height +";" + ROTRect.Top + ";" + ROTRect.Left + ";" + ROTRect.Width + ";" + ROTRect.Height + ";" + +MCRect.Top + ";" + MCRect.Left + ";" + MCRect.Width + ";" + MCRect.Height ;
            return new MemoryStream(Encoding.UTF8.GetBytes(styleText));
        }


        //Finding Most interesting Part 
        MIAFinder MIAfinder = new MIAFinder();
        
        void findMIA(String file)
        {

            ////  System.Drawing.Image imageFile  =   System.Drawing.Image.FromFile(file);
            //using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            //{
            //    using (Image original = Image.FromStream(fs))
            //    {
            //        Rectangle rect = MIAfinder.mostInterestingArea(original);

            //        Bitmap src = Image.FromFile(file) as Bitmap;
            //        Bitmap target = new Bitmap(rect.Width, rect.Height);

            //        using (Graphics g = Graphics.FromImage(target))
            //        {
            //            g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
            //                             rect,
            //                             GraphicsUnit.Pixel);
            //        }

            //        original.Save(file);

            //    }
            //}
            //

            //Another way to crop, produces out of memery since MIARect crosses image 
            //Note this will still produce an error 
        }

        public void crop (string number, string session)
        {
            ImgRecord r = sessions[session][int.Parse(number)];
            Bitmap image = new Bitmap("File" + r.file);
            Rectangle rect = new Rectangle(int.Parse(getParam("left")), int.Parse(getParam("top")), int.Parse(getParam("width")), int.Parse(getParam("height")));
            rect = trimRect(rect, image);

            Image x = image.Clone(rect, image.PixelFormat);
            image.Dispose();
            x.Save("File" + r.file);
            //Saves as png, so fix things.
            r.type = "image/png";
            int ix = r.name.LastIndexOf('.');
            if (ix != -1) r.name = r.name.Substring(0, ix + 1) + "png";
            x.Dispose();
        }

        Rectangle trimRect(Rectangle rect, Bitmap image)
        {
            if (rect.X > image.Width || rect.Y > image.Height
                || rect.Width <= 0 || rect.Height <= 0
                || rect.Right <= 0 || rect.Bottom <= 0)
            {
                rect.X = image.Width / 4;
                rect.Y = image.Height / 4;
                rect.Width = image.Width / 2;
                rect.Height = image.Height / 2;
                return rect;
            }
            if (rect.X < 0)
            {
                rect.Width += rect.X;
                rect.X = 0;
            }
            if (rect.Y < 0)
            {
                rect.Height += rect.Y;
                rect.Y = 0;
            }
            if (rect.X + rect.Width > image.Width) rect.Width = image.Width - rect.X;
            if (rect.Y + rect.Height > image.Height) rect.Height = image.Height - rect.Y;
            return rect;
        }

        String getParam(string key)
        {
            return WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters[key];
        }

        FileStream safeOpen(string path)
        {
            for (int i = 0; i < 50; i++)
            {
                try
                {
                    FileStream ret = new FileStream(path, FileMode.Open);
                    return ret;
                }
                catch (Exception) { }
                Thread.Sleep(100);
            }
            //Hail Mary, or just throw the real exception
            return new FileStream(path, FileMode.Open);
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
