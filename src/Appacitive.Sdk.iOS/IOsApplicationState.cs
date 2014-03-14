using System;
using Appacitive.Sdk.Internal;

namespace Appacitive.Sdk.iOS
{
	public class IOsApplicationState : IApplicationState
	{

		public static readonly IOsApplicationState Instance = new IOsApplicationState ();

		public APUser GetUser ()
		{
			return Global.User;
		}

		public string GetUserToken ()
		{
			return Global.UserToken;
		}

		public void SetUser (APUser user)
		{
			Global.User = user;
		}

		public void SetUserToken (string value)
		{
			Global.UserToken = value;
		}
	}

	public static class Global
	{
		public static APUser User {get; set;}

		public static string UserToken {get; set;}
	}
}

