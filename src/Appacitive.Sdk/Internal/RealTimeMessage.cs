using Appacitive.Sdk.Internal;
using Appacitive.Sdk.Realtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Realtime
{
    public abstract class RealTimeMessage
    {
        protected RealTimeMessage(string type)
        {
            _type = type;
        }

        private string _type;
        [JsonProperty("mt")]
        public string Code
        {
            get
            {
                return _type;
            }
            set
            {
                // Do nothing
            }
        }

        internal static Type GetType(string typeCode)
        {
            switch (typeCode)
            {
                case "1":
                    return typeof(SendToUsers);
                case "2":
                    return typeof(SendToHub);
                case "3":
                    return typeof(SubscribeToHubMessage);
                case "4":
                    return typeof(UnsubscribeFromHubMessage);
                case "5":
                    return typeof(SubscribeToObjectChangesMessage);
                case "6":
                    return typeof(UnsubscribeFromObjectChangesMessage);
                case "7":
                    return typeof(TypeUpdatedMessage);
                case "8":
                    return typeof(ObjectUpdatedMessage);
                case "9":
                    return typeof(NewNotificationMessage);
                default:
                    return null;

            }
        }

        public override string ToString()
        {
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            var bytes = serializer.Serialize(this);
            using (var memStream = new MemoryStream(bytes))
            {
                using( var reader = new StreamReader(memStream) )
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
