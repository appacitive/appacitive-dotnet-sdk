using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GetConnectionByEndpointResponse : ApiResponse
    {
        [JsonProperty("connection")]
        public APConnection Connection { get; set; }
    }
}
