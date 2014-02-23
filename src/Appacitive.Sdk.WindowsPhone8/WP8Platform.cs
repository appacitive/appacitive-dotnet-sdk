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
    public class WP8Platform : Platform, IDevicePlatform
    {
        protected override void InitializeContainer(Internal.IDependencyContainer container)
        {

            container
                .Register<IHttpConnector, HttpConnector>(() => new HttpConnector())
                .Register<IHttpFileHandler, WebClientHttpFileHandler>(() => new WebClientHttpFileHandler())
                .RegisterInstance<ILocalStorage, IsolatedLocalStorage>("wp8", IsolatedLocalStorage.Instance)
                ;
            var deviceState = new WPDeviceState(
                container.Build<ILocalStorage>("wp8"),
                container.Build<IJsonSerializer>());
            container.RegisterInstance<IDeviceState, WPDeviceState>("wp8", deviceState);
            var appState = new WPApplicationState(
                container.Build<ILocalStorage>("wp8"),
                container.Build<IJsonSerializer>());
            container.RegisterInstance<IApplicationState, WPApplicationState>("wp8", appState);
            
        }

        
        public static readonly Platform Instance = new WP8Platform();



        public IDeviceState DeviceState
        {
            get { return ObjectFactory.Build<IDeviceState>("wp8"); }
        }

        public IApplicationState ApplicationState
        {
            get { return ObjectFactory.Build<IApplicationState>("wp8"); }
        }
    }
}
