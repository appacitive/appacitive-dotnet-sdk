using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Realtime;

namespace Appacitive.Sdk.Services
{
    public class GetDeviceResponse : ApiResponse
    {
        [JsonProperty("device")]
        public Device Device { get; set; }

        public static GetDeviceResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<GetDeviceResponse>(bytes);
        }
    }
}
