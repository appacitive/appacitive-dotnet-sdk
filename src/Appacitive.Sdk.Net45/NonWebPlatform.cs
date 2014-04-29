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
    internal class NonWebPlatform : NetPlatform
    {
        public override IApplicationState ApplicationState
        {
            get { return new StaticApplicationState(); }
        }
    }
}
