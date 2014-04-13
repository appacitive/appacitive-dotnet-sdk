using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class UpdateGroupMembersRequest : PostOperation<UpdateGroupMembersResponse>
    {
        public UpdateGroupMembersRequest()
        {
            this.AddedUsers = new List<string>();
            this.RemovedUsers = new List<string>();
        }

        [JsonIgnore]
        public string Group { get; set; }

        [JsonProperty("add")]
        public List<string> AddedUsers { get; private set; }

        [JsonProperty("remove")]
        public List<string> RemovedUsers { get; private set; }

        protected override string GetUrl()
        {
            return Urls.For.UpdateGroupMembersRequest(this.Group, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
