using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NET40
namespace Appacitive.Sdk.Net40
#else
namespace Appacitive.Sdk.Net45
#endif
{
    public abstract class NetPlatform : IApplicationPlatform
    {
        public abstract IApplicationState ApplicationState { get; }

        public virtual void InitializeContainer(IDependencyContainer container)
        {
            container
                .Register<IHttpConnector, HttpConnector>(() => HttpConnector.Instance)
                .Register<IHttpFileHandler, WebClientHttpFileHandler>(() => new WebClientHttpFileHandler())
                .Register<ITraceWriter, DefaultTraceWriter>(() => DefaultTraceWriter.Instance)
                ;
        }

        public virtual void Init(IAppContextState context)
        {
        }
    }
}
