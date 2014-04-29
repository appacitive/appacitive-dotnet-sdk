using Appacitive.Sdk.Internal;
using Appacitive.Sdk.WindowsPhone7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appacitive.Sdk
{
    internal static class Platforms
    {   
        public static readonly IPlatform WP7 = WP7Platform.Instance;
    }
}
