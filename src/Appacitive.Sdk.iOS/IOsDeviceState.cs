using System;
using Appacitive.Sdk.Internal;
using System.Net.Http;

namespace Appacitive.Sdk.iOS
{
	public class IOsDeviceState : IDeviceState
	{
		public static IOsDeviceState Instance = new IOsDeviceState();

		public APDevice GetDevice ()
		{
			throw new NotImplementedException ();
		}

		public void SetDevice (APDevice device)
		{
			throw new NotImplementedException ();
		}

		public bool IsNetworkAvailable ()
		{
			NetworkStatus internetStatus = Reachability.InternetConnectionStatus();
			if (internetStatus == NetworkStatus.NotReachable)
				return false;
			else
				return true;
		}

		public DeviceType DeviceType 
		{
			get 
			{
				return DeviceType.iOS;
			}
		}

		public ILocalStorage LocalStorage 
		{
			get 
			{
				return ObjectFactory.Build<ILocalStorage> ("ios");
			}
		}

	}
}

