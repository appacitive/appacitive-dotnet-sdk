using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Realtime;

namespace Appacitive.Sdk.Services
{
    public class CreateObjectResponse : ApiResponse
    {
        [JsonProperty("article")]
        public APObject Object { get; set; }
    }
}
