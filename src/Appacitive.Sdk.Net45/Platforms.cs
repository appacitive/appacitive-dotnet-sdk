﻿using Appacitive.Sdk.Aspnet;
using Appacitive.Sdk.Internal;
using Appacitive.Sdk.Wcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static class Platforms
    {
        public static readonly Platform Net = new Net45Platform();
        public static readonly Platform Aspnet = new AspnetPlatform();
        public static readonly Platform Wcf = new WcfPlatform();
    }
}
