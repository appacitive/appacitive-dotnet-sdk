using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class ChangePasswordRequest : PostOperation<ChangePasswordResponse>
    {
        public ChangePasswordRequest() : base()
        {
            this.IdType = string.Empty; // String.empty indicates default type is id. This should probably be changed.
        }

        [JsonIgnore]
        public string UserId { get; set; }

        [JsonIgnore]
        public string IdType { get; set; }

        [JsonProperty("oldpassword")]
        public string OldPassword { get; set; }

        [JsonProperty("newpassword")]
        public string NewPassword { get; set; }

        protected override string GetUrl()
        {
            return Urls.For.ChangePassword(this.UserId, this.IdType, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }

    
}
