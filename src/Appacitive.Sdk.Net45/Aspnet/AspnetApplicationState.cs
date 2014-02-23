using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace Appacitive.Sdk.Aspnet
{
    public class AspnetApplicationState : IApplicationState
    {
        public static AspnetApplicationState Instance = new AspnetApplicationState();

        private HttpSessionState GetSession()
        {
            try
            {
                var session = HttpContext.Current.Session;
                if (session == null)
                    throw new AppacitiveRuntimeException("Http session not available.");
                return session;
            }
            catch (Exception ex)
            {
                throw new AppacitiveRuntimeException("Error acquiring asp.net session.", ex);
            }
        }

        public APUser GetUser()
        {
            var session = GetSession();
            var userBytes = session[SessionKeys.ForUser()] as byte[];
            if (userBytes == null || userBytes.Length == 0)
                return null;
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<APUser>(userBytes);
        }

        public string GetUserToken()
        {
            var token = this.GetSession()[SessionKeys.ForUserToken()] as string;
            if (string.IsNullOrWhiteSpace(token) == true)
                return null;
            else return token;
        }

        public void SetUser(APUser user)
        {
            if (user == null)
            {
                this.GetSession()[SessionKeys.ForUser()] = null;
            }
            else
            {
                var serializer = ObjectFactory.Build<IJsonSerializer>();
                var bytes = serializer.Serialize(user);
                this.GetSession()[SessionKeys.ForUser()] = bytes;
            }
        }

        public void SetUserToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value) == true)
                value = null;
            this.GetSession()[SessionKeys.ForUserToken()] = value;

        }
    }

    internal static class SessionKeys
    {
        private static readonly string Namespace = "http://www.appacitive.com/sdk/net/asp/2014/01/";

        private static string GetName(string suffix)
        {
            return Namespace + suffix;
        }

        internal static string ForUser()
        {
            return GetName("currentUser");
        }

        internal static int ForUserToken()
        {
            throw new NotImplementedException();
        }
    }

}
