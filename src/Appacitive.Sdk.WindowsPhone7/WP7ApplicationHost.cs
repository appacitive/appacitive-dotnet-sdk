using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Appacitive.Sdk.Realtime;

namespace Appacitive.Sdk.WindowsPhone7
{

    public static class WP7
    {
        public static readonly IApplicationHost Instance = new WP7ApplicationHost();
    }

    public class WP7ApplicationHost : IApplicationHost
    {
        
        public void InitializeContainer(IDependencyContainer container)
        {
            
            container
                .Register <IRealTimeTransport, SignalRTransport>( () => new SignalRTransport() )
                .Register<IHttpConnector, HttpConnector>(() => new HttpConnector())
                .Register<IHttpFileHandler, WebClientHttpFileHandler>(() => new WebClientHttpFileHandler())
                .Register<IExceptionFactory, ExceptionFactory>(() => new ExceptionFactory());
        }
    }
}
