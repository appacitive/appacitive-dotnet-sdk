using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    internal class RealtimeEvents
    {
        public static readonly string Create = "create";
        public static readonly string Update = "update";
        public static readonly string Delete = "delete";

        internal static void ValidateEvent(string eventType)
        {
            var isValid =
                eventType.Equals(Create, StringComparison.OrdinalIgnoreCase) == true ||
                eventType.Equals(Update, StringComparison.OrdinalIgnoreCase) == true ||
                eventType.Equals(Delete, StringComparison.OrdinalIgnoreCase) == true;
            if (isValid == false)
                throw new Exception("Event type must by create, update or delete.");
        }
    }
}
