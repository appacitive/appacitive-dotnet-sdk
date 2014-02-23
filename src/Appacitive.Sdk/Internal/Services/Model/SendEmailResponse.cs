using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Appacitive.Sdk.Services
{
    public class SendEmailResponse : ApiResponse
    {
        [JsonProperty("email")]
        public Email Email { get; set; }
    }
}
