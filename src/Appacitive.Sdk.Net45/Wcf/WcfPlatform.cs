using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET40
using Appacitive.Sdk.Net40;
#else
using Appacitive.Sdk.Net45;
#endif

namespace Appacitive.Sdk.Wcf
{
    public class WcfPlatform : NetPlatform, IApplicationPlatform
    {
        public static new readonly WcfPlatform Instance = new WcfPlatform();

        public override IApplicationState ApplicationState
        {
            get { return WcfApplicationState.Instance; }
        }
    }
}
