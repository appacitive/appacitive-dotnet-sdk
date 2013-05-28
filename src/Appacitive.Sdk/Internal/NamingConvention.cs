using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    internal static class NamingConvention
    {
        public static EventType FromString(string eventCode)
        {
            switch (eventCode.ToLower())
            {
                case "cc":
                    return EventType.ConnectionCreate;
                case "cu":
                    return EventType.ConnectionUpdate;
                case "cd":
                    return EventType.ConnectionDelete;
                case "ac":
                    return EventType.ArticleCreate;
                case "au":
                    return EventType.ArticleUpdate;
                case "ad":
                    return EventType.ArticleDelete;
                default:
                    throw new Exception("Unsupported event code + " + eventCode + ".");
            }
        }

        public static string ToString(EventType type)
        {
            switch (type)
            {
                case EventType.ArticleCreate:
                    return "ac";
                case EventType.ArticleUpdate:
                    return "au";
                case EventType.ArticleDelete:
                    return "ad";
                case EventType.ConnectionCreate:
                    return "cc";
                case EventType.ConnectionUpdate:
                    return "cu";
                case EventType.ConnectionDelete:
                    return "cd";
                default:
                    throw new Exception("Unsupported event type " + type.ToString() + ".");
            }
        }
    }
}
