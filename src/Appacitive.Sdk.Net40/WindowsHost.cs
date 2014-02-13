using Appacitive.Sdk.Interfaces;
using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
                .Register<IHttpConnector, HttpConnector>( () => HttpConnector.Instance)
                .Register<IHttpFileHandler, WebClientHttpFileHandler>(() => new WebClientHttpFileHandler())
                .Register<ITraceWriter, DefaultTraceWriter>( () => DefaultTraceWriter.Instance) 
                ;
        }
    }

    internal class InstanceCache<T>
    {
        public InstanceCache( Func<T> createNew)
        {
            var t = createNew();
            this.Instance = () => t;
        }

        public Func<T> Instance { get; private set; }
    }

    public static class Extensions
    {
        public static Func<T> AsSingleton<T>(this Func<T> func)
        {
            return new InstanceCache<T>(func).Instance;
        }
    }
}


