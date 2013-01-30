using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class CreateUserResponse : ApiResponse
    {
        [JsonProperty("user")]
        public User User { get; set; }

        public static CreateUserResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<CreateUserResponse>(bytes);
        }
    }
}
