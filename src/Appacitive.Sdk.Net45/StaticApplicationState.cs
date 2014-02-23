using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NET40
namespace Appacitive.Sdk.Net40
#else
namespace Appacitive.Sdk.Net45
#endif
{
    public class StaticApplicationState : IApplicationState
    {
        private static APUser _user;

        public APUser GetUser()
        {
            return _user;
        }

        private static string _token;
        public string GetUserToken()
        {
            return _token;
        }

        public void SetUser(APUser user)
        {
            _user = user;
        }

        public void SetUserToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value) == true)
                _token = null;
            else _token = value;
        }
    }
}
