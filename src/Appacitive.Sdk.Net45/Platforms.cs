using Appacitive.Sdk.Internal;
using Appacitive.Sdk.Aspnet;
using Appacitive.Sdk.Wcf;
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

namespace Appacitive.Sdk
{
    public static class Platforms
    {
		public static readonly Platform Aspnet = new AspnetPlatform();
        public static readonly Platform Wcf = new WcfPlatform();
        public static readonly Platform NonWeb = new NonWebPlatform();
    }
}
