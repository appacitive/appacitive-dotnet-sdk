using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.WinRT
{
    public class WindowsHost : IApplicationHost
    {
        void IApplicationHost.InitializeContainer(IDependencyContainer container)
        {
            container
                .Register<IHttpConnector, HttpConnector>(() => new HttpConnector())
                .Register<IHttpFileHandler, WebClientHttpFileHandler>(() => new WebClientHttpFileHandler())
                ;
        }
    }
}
