using System;
using Appacitive.Sdk.Internal;

namespace Appacitive.Sdk.iOS
{
	public class IOsPlatform : Platform, IDevicePlatform
	{
		protected override void InitializeContainer (IDependencyContainer container)
		{
			var localStorage = new CachedLocalStorage (new IOsLocalStorage ());
			container
				.RegisterInstance<ILocalStorage, CachedLocalStorage> ("ios", localStorage)
				.RegisterInstance<IHttpConnector, HttpConnector> ( HttpConnector.Instance)
				.Register<IHttpFileHandler, WebClientHttpFileHandler> ( () => new WebClientHttpFileHandler() );
		}

		public IDeviceState DeviceState {
			get 
			{
				return IOsDeviceState.Instance;
			}
		}

		public IApplicationState ApplicationState 
		{
			get 
			{
				return IOsApplicationState.Instance;
			}
		}
	}
}

