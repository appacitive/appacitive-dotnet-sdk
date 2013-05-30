using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public interface IUpstreamMessage
    {
        string UserToken { get; set;  }
    }
}
