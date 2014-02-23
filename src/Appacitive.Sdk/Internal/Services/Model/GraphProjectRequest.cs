using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GraphProjectRequest : PostOperation<GraphProjectResponse>
    {
        [JsonIgnore]
        public string Query { get; set; }

        [JsonProperty("ids")]
        public List<string> Ids { get; set; }

        [JsonProperty("placeholders")]
        public IDictionary<string, string> Placeholders { get; set; }
        
        protected override string GetUrl()
        {
            return Urls.For.GraphProject(this.Query, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
