using Appacitive.Sdk.Internal;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.WindowsPhone7
{
    public class WP7Platform : IDevicePlatform
    {
        public static readonly IPlatform Instance = new WP7Platform();

        public void InitializeContainer(Internal.IDependencyContainer container)
        {

            container
                .Register<IHttpConnector, HttpConnector>(() => new HttpConnector())
                .Register<IHttpFileHandler, WebClientHttpFileHandler>(() => new WebClientHttpFileHandler())
                .RegisterInstance<ILocalStorage, IsolatedLocalStorage>("wp7", IsolatedLocalStorage.Instance)
                ;
            var deviceState = new WPDeviceState(
                container.Build<ILocalStorage>("wp7"),
                container.Build<IJsonSerializer>());
            container.RegisterInstance<IDeviceState, WPDeviceState>("wp7", deviceState);
            var appState = new WPApplicationState(
                container.Build<ILocalStorage>("wp7"),
                container.Build<IJsonSerializer>());
            container.RegisterInstance<IApplicationState, WPApplicationState>("wp7", appState);

        }

        public void Init(AppContext context)
        {
        }

        public IDeviceState DeviceState
        {
            get { return ObjectFactory.Build<IDeviceState>("wp7"); }
        }

        public IApplicationState ApplicationState
        {
            get { return ObjectFactory.Build<IApplicationState>("wp7"); }
        }
    }
}
