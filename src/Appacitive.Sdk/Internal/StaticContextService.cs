using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class StaticContextService : IContextService
    {
        public static readonly IContextService Instance = new StaticContextService();
        private StaticContextService() { }
        private static APUser _user;
        private static APDevice _device;
        private static string _userToken;

        public APUser GetUser()
        {
            return _user;
        }

        public string GetUserToken()
        {
            return _userToken;
        }

        public APDevice GetDevice()
        {
            return _device;
        }

        public void SetDevice(APDevice device)
        {
            _device = device;
        }

        public void SetUser(APUser user)
        {
            _user = user;
        }

        public void SetUserToken(string value)
        {
            _userToken = value;
        }
    }
}
