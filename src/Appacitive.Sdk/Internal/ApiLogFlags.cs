using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    [Flags]
    public enum ApiLogFlags
    {
        None = 1,
        SuccessfulCalls = 2,
        FailedCalls = 4,
        SlowLogs = 8,
        Conditional = 16,
        Everything = SuccessfulCalls | FailedCalls | SlowLogs
    }
}
