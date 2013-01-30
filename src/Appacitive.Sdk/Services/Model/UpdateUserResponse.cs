using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class UpdateUserResponse : ApiResponse
    {
        public static UpdateUserResponse Parse(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<UpdateUserResponse>(data);
        }

        [JsonProperty("user")]
        public User User { get; set; }
    }
}
