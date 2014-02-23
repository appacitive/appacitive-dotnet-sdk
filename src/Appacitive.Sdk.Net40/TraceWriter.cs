using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class DefaultTraceWriter : ITraceWriter
    {
        public static readonly DefaultTraceWriter Instance = new DefaultTraceWriter();

        public Task WritelineAsync(string trace)
        {
            Trace.WriteLine(trace);
            return TaskEx.FromResult(true);
        }
    }
}
