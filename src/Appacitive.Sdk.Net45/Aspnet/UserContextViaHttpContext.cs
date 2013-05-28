using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Appacitive.Sdk.Aspnet
{
    public class UserContextViaHttpContext : IUserContext
    {
        public static readonly IUserContext Instance = new UserContextViaHttpContext();

        private static readonly string TokenKey = "DF7A2727-8081-48B8-9AB3-660A02E47680_UserToken";

        public void SetUserToken(string authToken)
        {
            var context = HttpContext.Current;
            if (context == null)
                throw new Exception("Http context not available.");
            context.Items[TokenKey] = authToken;
        }

        public string GetUserToken()
        {
            var context = HttpContext.Current;
            if (context == null)
                throw new Exception("Http context not available.");
            var token = context.Items[TokenKey];
            return token == null ? null : token.ToString();
        }
    }
}
