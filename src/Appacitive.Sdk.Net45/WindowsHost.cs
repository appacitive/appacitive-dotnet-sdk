using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Net45
{
    public class WindowsHost : IApplicationHost
    {
        public static readonly IApplicationHost Instance = new WindowsHost();

        void IApplicationHost.InitializeContainer(IDependencyContainer container)
        {
            this.InitializeContainer(container);
        }

        protected virtual void InitializeContainer(IDependencyContainer container)
        {
            container
                .Register<IHttpConnector, HttpConnector>(() => new HttpConnector())
                .Register<IHttpFileHandler, WebClientHttpFileHandler>(() => new WebClientHttpFileHandler())
                .Register<IExceptionFactory, ExceptionFactory>( () => ExceptionFactory.Instance )
                .Register<IRealTimeTransport, SignalRTransport>(() => new SignalRTransport())
                ;
        }
    }
}
