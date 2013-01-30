using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class CreateSessionRequest : ApiRequest
    {
        public CreateSessionRequest()
            : base(null, Environment.Sandbox, null)
        {
        }

        [JsonProperty("apikey")]
        public string ApiKey { get; set; }

        [JsonProperty("isnonsliding")]
        public bool IsNonSliding { get; set; }

        [JsonProperty("windowtime")]
        public int WindowTime { get; set; }

        [JsonProperty("usagecount")]
        public int UsageCount { get; set; }
    }
}
