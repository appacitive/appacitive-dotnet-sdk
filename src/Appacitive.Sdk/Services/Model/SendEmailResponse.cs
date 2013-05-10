using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Interfaces;

namespace Appacitive.Sdk.Services
{
    public class SendEmailResponse : ApiResponse
    {
        [JsonProperty("email")]
        public Email Email { get; set; }

        public static SendEmailResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<SendEmailResponse>(bytes);
        }
    }
}
