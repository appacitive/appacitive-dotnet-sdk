using Appacitive.Sdk.Internal;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.WindowsPhone8
{
    public class WP8Platform : Platform
    {
        public WP8Platform()
        {   
        }

        public void Initialize()
        {
            var channel = HttpNotificationChannel.Find("");
            
        }

        void channel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            
        }

        public override void InitializeContainer(Internal.IDependencyContainer container)
        {

            container
                .Register<IHttpConnector, HttpConnector>(() => new HttpConnector())
                .Register<IHttpFileHandler, WebClientHttpFileHandler>(() => new WebClientHttpFileHandler())
                .RegisterInstance<ILocalStorage, IsolatedLocalStorage>("wp8", IsolatedLocalStorage.Instance)
                ;
            var contextService = new WPContextService(
                container.Build<ILocalStorage>("wp8"),
                container.Build<IJsonSerializer>());
            container.RegisterInstance<IContextService, WPContextService>("wp8", contextService);
            
        }

        public override bool IsNetworkAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }

        public override IContextService ContextService
        {
            get { return ObjectFactory.Build<IContextService>("wp8");}
        }

        public override ILocalStorage LocalStorage
        {
            get { return IsolatedLocalStorage.Instance; }
        }

        public static readonly Platform Instance = new WP8Platform();
    }
}
