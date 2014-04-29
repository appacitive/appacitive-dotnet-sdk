using Appacitive.Sdk.WindowsPhone8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    internal static class Platforms
    {
        public static readonly IPlatform WP8 = WP8Platform.Instance;
    }
}
