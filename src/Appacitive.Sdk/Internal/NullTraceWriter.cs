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
        #if ( WINDOWS_PHONE7 || NET40)
        private static readonly Task CachedTask = TaskEx.FromResult(true);
        #endif

        public Task WritelineAsync(string trace)
        {
            #if ( WINDOWS_PHONE7 || NET40)
            return CachedTask;
            #else
            return Task.FromResult(true);
            #endif
        }
    }
}
