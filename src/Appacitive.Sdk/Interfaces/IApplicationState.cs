using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public interface IApplicationState
    {
        APUser GetUser();

        string GetUserToken();

        void SetUser(APUser user);

        void SetUserToken(string value);
    }
}

