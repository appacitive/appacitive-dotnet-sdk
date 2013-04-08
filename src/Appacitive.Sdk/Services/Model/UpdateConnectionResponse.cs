using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Interfaces;

namespace Appacitive.Sdk.Services
{
    public class UpdateConnectionResponse : ApiResponse
    {
        public static UpdateConnectionResponse Parse(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<UpdateConnectionResponse>(data);
        }

        [JsonProperty("connection")]
        public Connection Connection { get; set; }
    }
}
