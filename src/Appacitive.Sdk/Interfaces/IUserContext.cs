using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Interfaces
{
    public interface IUserContext
    {
        void SetUserToken(string authToken);

        string GetUserToken();
    }
}
