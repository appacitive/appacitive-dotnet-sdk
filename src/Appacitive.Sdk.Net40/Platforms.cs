using Appacitive.Sdk.Aspnet;
using Appacitive.Sdk.Internal;
using Appacitive.Sdk.Wcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appacitive.Sdk
{
    public static class Platforms
    {
        public static readonly Platform Net = new Net40Platform();
        public static readonly Platform Aspnet = new AspnetPlatform();
        public static readonly Platform Wcf = new WcfPlatform();
    }
}
