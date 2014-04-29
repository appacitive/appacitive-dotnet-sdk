using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Internal;
#if NET40
using Appacitive.Sdk.Net40;
#else
using Appacitive.Sdk.Net45;
#endif

namespace Appacitive.Sdk.Aspnet
{
    public class AspnetPlatform : NetPlatform
    {
        public static readonly AspnetPlatform Instance = new AspnetPlatform();

        public override IApplicationState ApplicationState
        {
            get { return AspnetApplicationState.Instance; }
        }
    }
}
