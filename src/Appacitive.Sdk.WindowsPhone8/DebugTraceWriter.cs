using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_PHONE
namespace Appacitive.Sdk.WindowsPhone8
#elif WINDOWS_PHONE7
namespace Appacitive.Sdk.WindowsPhone7
#endif
{
    internal class DebugTraceWriter : ITraceWriter
    {
        public Task WritelineAsync(string trace)
        {
            return Task.Factory.StartNew(() => Debug.WriteLine(trace));
        }
    }
}
