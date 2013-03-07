using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Interfaces;

namespace Appacitive.Sdk.Services
{
    public class Status
    {
        public static Status Parse(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<Status>(data);
        }

        [JsonProperty("referenceid")]
        public string ReferenceId { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("faulttype")]
        public string FaultType { get; set; }

        [JsonProperty("additionalmessages")]
        public List<string> AdditionalMessages { get; set; }

        [JsonIgnore]
        public bool IsSuccessful
        {
            get { return this.Code == "200"; }
        }
    }
}
