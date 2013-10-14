using Appacitive.Sdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    internal class NullTraceWriter : ITraceWriter
    {
        public static readonly NullTraceWriter Instance = new NullTraceWriter();
#if WINDOWS_PHONE7
        private static readonly Task CachedTask = new Task(() => { });  
#endif

        public Task WritelineAsync(string trace)
        {
#if !WINDOWS_PHONE7
            return Task.FromResult(true);
#else
            return CachedTask;  
#endif
        }
    }
}
