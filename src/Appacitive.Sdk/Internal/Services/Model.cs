using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class Session
    {
        [JsonProperty("sessionkey")]
        public string SessionKey { get; set; }

        [JsonProperty("usagecount")]
        public long UsageCount { get; set; }

        [JsonProperty("isnonsliding")]
        public bool IsNonSliding { get; set; }

        [JsonProperty("windowtime")]
        public int WindowTime { get; set; }
    }
}
