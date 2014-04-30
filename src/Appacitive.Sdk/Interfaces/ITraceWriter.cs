using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    /// <summary>
    /// TraceWriter interface for logging SDK traces.
    /// </summary>
    public interface ITraceWriter
    {
        Task WritelineAsync(string trace);
    }
}
