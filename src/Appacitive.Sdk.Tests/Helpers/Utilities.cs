using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class Utilities
    {
        public static async Task Delay(int milliseconds)
        {
            #if NET40
            await TaskEx.Delay(milliseconds);
            #else
            await Task.Delay(milliseconds);
            #endif
        }
    }
}
