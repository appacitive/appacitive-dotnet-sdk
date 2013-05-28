using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class StaticUserContext : IUserContext
    {
        public void SetUserToken(string authToken)
        {
            AppacitiveContext.GlobalUserToken = authToken;
        }

        public string GetUserToken()
        {
            return AppacitiveContext.GlobalUserToken;
        }
    }
}
