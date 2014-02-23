using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.WindowsPhone7
{

    public class WP7Platform : Platform
    {
        public static Platform Instance = new WP7Platform();

        public override void InitializeContainer(IDependencyContainer container)
        {
            container
                .Register<IHttpConnector, HttpConnector>(() => new HttpConnector())
                .Register<IHttpFileHandler, WebClientHttpFileHandler>(() => new WebClientHttpFileHandler())
                .RegisterInstance<ILocalStorage, IsolatedLocalStorage>("wp7", IsolatedLocalStorage.Instance)
                ;
            var contextService = new WPContextService(
                container.Build<ILocalStorage>("wp7"),
                container.Build<IJsonSerializer>());
            container.RegisterInstance<IContextService, WPContextService>("wp7", contextService);
        }

        public override bool IsNetworkAvailable()
        { 
            return NetworkInterface.GetIsNetworkAvailable();
        }

        public override IContextService ContextService
        {
            get { return ObjectFactory.Build<IContextService>("wp7"); }
        }

        public override ILocalStorage LocalStorage
        {
            get { return IsolatedLocalStorage.Instance; }
        }
    }
}
