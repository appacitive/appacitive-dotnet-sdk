using Appacitive.Sdk.Realtime;
using Appacitive.Sdk.Net45;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Appacitive.Sdk.Aspnet
{
    public class AspnetUserContext : IUserContext
    {
        public void SetUserToken(string authToken)
        {
            var principal = GetPrincipal();
            principal.AuthToken = authToken;
        }

        public string GetUserToken()
        {
            var principal = GetPrincipal();
            return principal.AuthToken;
        }

        private IAppacitiveUserPrincipal GetPrincipal()
        {
            var principal = HttpContext.Current.User as IAppacitiveUserPrincipal;
            if (principal == null)
                throw new Exception("Unsupported user principal. User principal must implement IAppacitiveUserPrincipal");
            return principal;
        }
    }
}
