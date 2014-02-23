using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Appacitive.Sdk.Services
{
    public class SendPushNotificationResponse : ApiResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
