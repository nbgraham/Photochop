﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;

namespace ConsoleApplication1
{
    [ServiceContract]
    public interface IMyService
    {
        [OperationContract]
        [WebGet(UriTemplate = "")]
        Stream sayHello();

        [OperationContract]
        [WebGet(UriTemplate = "servable/{*path}")]
        Stream serveFile(string path);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "initUpload")]
        Stream initUpload(Stream body);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "nextUpload/{session}")]
        Stream nextUpload(string session, Stream body);

        [OperationContract]
        [WebGet(UriTemplate = "File{number}/{session}")]
        Stream fileGet(string number, string session);

        [OperationContract]
        [WebGet(UriTemplate = "crop/{number}/{session}")]
        void crop(string number, string session);

        //[OperationContract]
        //[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "findMIA")]
        //Stream findMIA(string session, Stream body);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "getMIA/{index}/{session}")]
        Stream getMIA(string index, string session);
    }
}
