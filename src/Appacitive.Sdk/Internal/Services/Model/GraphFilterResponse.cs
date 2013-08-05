using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GraphFilterResponse : ApiResponse
    {
        [JsonProperty("ids")]
        public List<string> Ids {get; set;}
    }
}
