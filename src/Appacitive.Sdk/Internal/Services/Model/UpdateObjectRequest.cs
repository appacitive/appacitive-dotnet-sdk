using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class UpdateObjectRequest : PostOperation<UpdateObjectResponse>
    {
        public UpdateObjectRequest() : base()
        {
            this.PropertyUpdates = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.AttributeUpdates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.AddedTags = new List<string>();
            this.RemovedTags = new List<string>();
            this.Revision = 0;
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
            return Urls.For.UpdateObject(this.Type, this.Id, this.Revision, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
