using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    internal static class DefaultRegistrations
    {
        public static void ConfigureContainer(IDependencyContainer container)
        {
            container
                    .Register<IJsonSerializer, JsonDotNetSerializer>(() => new JsonDotNetSerializer())
                    .Register<ITraceWriter, NullTraceWriter>(() => NullTraceWriter.Instance)
                    ;
        }
    }
}
