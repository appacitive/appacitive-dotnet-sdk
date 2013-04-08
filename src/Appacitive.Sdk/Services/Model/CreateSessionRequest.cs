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
            : base(null, null, Environment.Sandbox, null)
        {
        }

        [JsonProperty("apikey")]
        public string APIKey { 
            get { return this.ApiKey; }
            set { this.ApiKey = value; }
        }

        [JsonProperty("isnonsliding")]
        public bool IsNonSliding { get; set; }

        [JsonProperty("windowtime")]
        public int WindowTime { get; set; }

        [JsonProperty("usagecount")]
        public int UsageCount { get; set; }
    }
}
