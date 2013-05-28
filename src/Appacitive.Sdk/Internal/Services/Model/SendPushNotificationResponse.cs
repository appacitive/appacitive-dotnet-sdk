using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Realtime;

namespace Appacitive.Sdk.Services
{
    public class SendPushNotificationResponse : ApiResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public static SendPushNotificationResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<SendPushNotificationResponse>(bytes);
        }
    }
}
