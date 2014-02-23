using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class UpdateConnectionRequest : PostOperation<UpdateConnectionResponse>
    {
        public UpdateConnectionRequest()
        {
            this.PropertyUpdates = new Dictionary<string, object>();
            this.AttributeUpdates = new Dictionary<string, string>();
            this.AddedTags = new List<string>();
            this.RemovedTags = new List<string>();
        }

        [JsonIgnore]
        public int Revision { get; set; }

        [JsonIgnore]
        public string Id { get; set; }

        [JsonIgnore]
        public string Type { get; set; }

        [JsonIgnore]
        public IDictionary<string, object> PropertyUpdates { get; private set; }

        [JsonIgnore]
        public IDictionary<string, string> AttributeUpdates { get; private set; }

        [JsonIgnore]
        public List<string> AddedTags { get; private set; }

        [JsonIgnore]
        public List<string> RemovedTags { get; private set; }

        protected override string GetUrl()
        {
            return Urls.For.UpdateConnection(this.Type, this.Id, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
