using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class UpdateUserRequest : PostOperation<UpdateUserResponse>
    {
        public UpdateUserRequest() : base()
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
        public string UserId { get; set; }

        [JsonIgnore]
        public string IdType { get; set; }

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
            return Urls.For.UpdateUser(this.UserId, this.IdType, this.Revision, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
