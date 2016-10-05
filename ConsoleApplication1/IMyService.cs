using System;
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
    }
}
