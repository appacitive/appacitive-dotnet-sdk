using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GraphFilterRequest : PostOperation<GraphFilterResponse>
    {
        [JsonIgnore]
        public string Query { get; set; }

        [JsonProperty("placeholders")]
        public IDictionary<string, string> Placeholders { get; set; }
        
        protected override string GetUrl()
        {
            return Urls.For.GraphFilter(this.Query, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
