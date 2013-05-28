using Appacitive.Sdk.Realtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class ValidateUserSessionResponse : ApiResponse
    {
        [JsonProperty("result")]
        public bool Result { get; set; }

        public static ValidateUserSessionResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<ValidateUserSessionResponse>(bytes);
        }
    }
}
