using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.WinRT
{
    public interface IAppacitiveUserPrincipal
    {
        string AuthToken { get; set; }
    }
}
