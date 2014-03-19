using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Appacitive.Sdk.Wcf
{
    internal static class AppSession
    {
        private static readonly string SessionCookieName = "F47334C5F31C497B86C7FD9466A86A99";

        public static string Setup()
        {
            string sessionId = null;
            var isSessionAvailable = TryGetSessionId(out sessionId);
            if( isSessionAvailable == false )
            {
                sessionId = GenerateSessionId();
                return sessionId;
            }

            var userToken = GetUserTokenForSession(sessionId);
            if (string.IsNullOrWhiteSpace(userToken) == false)
                App.LoginAsync(new UserTokenCredentials(userToken)).Wait();
            return sessionId;
        }

        private static string GenerateSessionId()
        {
            return Guid.NewGuid().ToString();
        }

        private static bool TryGetSessionId(out string sessionId)
        {
            sessionId = Cookies.GetRequestCookie(SessionCookieName);
            if (string.IsNullOrWhiteSpace(sessionId) == true)
            {
                sessionId = null;
                return false;
            }
            else return true;
        }

        private static string GetUserTokenForSession(string sessionId)
        {
            var session = ObjectFactory.Build<ISession>();
            var userToken = session[GetUserTokenKey(sessionId)] as string;
            return userToken;
        }

        private static string GetUserTokenKey(string sessionId)
        {
            return sessionId + "-usertoken";
        }

        private static void PeristUserSession(string sessionId, string userToken)
        {
            var session = ObjectFactory.Build<ISession>();
            session[GetUserTokenKey(sessionId)] = userToken;
        }

        private static bool IsLoggedIn(out string userToken)
        {
            userToken = null;
            var user = App.Current.GetCurrentUser();
            if (user == null || string.IsNullOrWhiteSpace(user.UserToken) == true)
            {
                userToken = null;
                return false;
            }
            else
            {
                userToken = user.UserToken;
                return true;
            }
        }

        public static void Persist(string sessionId)
        {
            string userToken = string.Empty;
            bool isLoggedIn = IsLoggedIn(out userToken);
            if (isLoggedIn == false)
                TearDownSession(sessionId);
            else
                PersistSession(sessionId, userToken);
        }

        private static void PersistSession(string sessionId, string userToken)
        {
            // Persist the user token in session
            PeristUserSession(sessionId, userToken);
            // Return the session id in the cookie.
            Cookies.SetResponseCookie(SessionCookieName, sessionId);
        }

        private static void TearDownSession(string sessionId)
        {
            // Remove the sessionid from session.
            var session = ObjectFactory.Build<ISession>();
            session.Remove(GetUserTokenKey(sessionId));
            // Remove the cookie
            var context = WebOperationContext.Current;
            if (context == null || context.IncomingRequest == null)
                return;
            var expiry = DateTime.UtcNow.AddDays(-2);
            context.OutgoingResponse.Headers[HttpResponseHeader.SetCookie] = new Cookie(SessionCookieName, sessionId, expiry).ToString();
        }
    }

    

    internal static class Cookies
    {
        public static string GetRequestCookie(string key)
        {
            var context = WebOperationContext.Current;
            if (context == null || context.IncomingRequest == null)
                return null;
            var cookiesString = context.IncomingRequest.Headers[HttpRequestHeader.Cookie];
            if (string.IsNullOrWhiteSpace(cookiesString) == true) return null;
            var cookies = ParseCookies(cookiesString);
            return cookies[key];
        }

        private static NameValueCollection ParseCookies(string cookiesString)
        {
            NameValueCollection cookies = new NameValueCollection();
            if (string.IsNullOrWhiteSpace(cookiesString) == true)
                return cookies;

            // Assumed format is that cookies are returned as k1=v1; k2=v2;..
            cookiesString = HttpUtility.UrlDecode(cookiesString);
            var tokens = cookiesString.Split(';');
            
            for (int i = 0; i < tokens.Length; i++)
            {
                var index = tokens[i].IndexOf("=");
                if (index == -1) continue;
                cookies[tokens[i].Substring(0, index)] = tokens[i].Substring(index + 1);
            }
            return cookies;
        }

        internal static void SetResponseCookie(string sessionCookieName, string sessionId)
        {
            var context = WebOperationContext.Current;
            if (context == null || context.IncomingRequest == null)
                return;
            context.OutgoingResponse.Headers[HttpResponseHeader.SetCookie] = new Cookie(sessionCookieName, sessionId).ToString();
        }
    }

    internal class Cookie
    {
        public Cookie(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public Cookie(string name, string value, DateTime expiry)
        {
            this.Name = name;
            this.Value = value;
            this.Expiry = expiry;
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime? Expiry { get; set; }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            // Set-Cookie: LSID=DQAAAK…Eaem_vYg; Path=/accounts; Expires=Wed, 13 Jan 2021 22:23:01 GMT; Secure; HttpOnly
            buffer.Append(this.Name).Append("=").Append(this.Value).Append(";");
            if( this.Expiry != null && this.Expiry.HasValue == true )
                buffer.Append(" Expires=").Append(Expiry.Value.ToUniversalTime().ToString("R")).Append(";");
            buffer.Append(" Path=/;");
            buffer.Append(" HttpOnly");
            return buffer.ToString();
        }
    }
}
