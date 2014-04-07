using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class UpdateDeviceRequest : PostOperation<UpdateDeviceResponse>, IObjectUpdateRequest
    {
        public UpdateDeviceRequest()
            : base()
        {
            this.PropertyUpdates = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.AttributeUpdates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.AllowClaims = new List<Claim>();
            this.DenyClaims = new List<Claim>();
            this.ResetClaims = new List<ResetRequest>();
            this.AddedTags = new List<string>();
            this.RemovedTags = new List<string>();
            this.Revision = 0;
        }

        [JsonIgnore]
        public bool ReturnAcls { get; set; }

        [JsonIgnore]
        public List<Claim> AllowClaims { get; private set; }

        [JsonIgnore]
        public List<Claim> DenyClaims { get; private set; }

        [JsonIgnore]
        public List<ResetRequest> ResetClaims { get; private set; }

        [JsonIgnore]
        public int Revision { get; set; }

        [JsonIgnore]
        public string Id { get; set; }

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

        public string Type
        {
            get { return "device"; }
            set { return;  }
        }

        protected override string GetUrl()
        {
            return Urls.For.GetDevice(this.Id, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
