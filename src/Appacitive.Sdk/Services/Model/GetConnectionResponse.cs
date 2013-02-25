using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class GetConnectionResponse : ApiResponse
    {
        [JsonProperty("connection")]
        public Connection Connection { get; set; }

        public static GetConnectionResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<GetConnectionResponse>(bytes);
        }
    }
}
