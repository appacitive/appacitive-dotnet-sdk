using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class BulkDeleteObjectRequest : PostOperation<BulkDeleteObjectResponse>
    {
        [JsonIgnore]
        public string Type { get; set; }

        [JsonProperty("idlist")]
        public List<string> ObjectIds { get; set; }



        protected override string GetUrl()
        {
            return Urls.For.BulkDeleteObjects(this.Type, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
