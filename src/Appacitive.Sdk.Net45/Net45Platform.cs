using Appacitive.Sdk.Internal;
using Appacitive.Sdk.Net45;
using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class Net45Platform : Platform
    {
        public override void InitializeContainer(IDependencyContainer container)
        {
            container
                .Register<IHttpConnector, HttpConnector>(() => HttpConnector.Instance)
                .Register<IHttpFileHandler, WebClientHttpFileHandler>(() => new WebClientHttpFileHandler())
                .Register<ITraceWriter, DefaultTraceWriter>(() => DefaultTraceWriter.Instance)
                ;
        }

        public override bool IsNetworkAvailable()
        {
            // This platform instance represents a server side non-web
            // application running on .NET 4.5
            // This will always return true.
            return true;
        }

        public override IContextService ContextService
        {
            get { return StaticContextService.Instance; }
        }

        public override ILocalStorage LocalStorage
        {
            get { throw new NotSupportedException("Local storage is supported on .NET 4.0 platform."); }
        }
    }
}
