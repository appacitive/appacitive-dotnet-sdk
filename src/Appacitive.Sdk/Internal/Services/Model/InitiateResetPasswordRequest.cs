using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class InitiateResetPasswordRequest : PostOperation<InitiateResetPasswordResponse>
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("subject")]
        public string EmailSubject { get; set; }

        protected override string GetUrl()
        {
            return Urls.For.InitiateResetPassword(this.CurrentLocation, this.DebugEnabled, this.Verbosity);
        }
    }

    public class InitiateResetPasswordResponse : ApiResponse
    {   
    }
}
