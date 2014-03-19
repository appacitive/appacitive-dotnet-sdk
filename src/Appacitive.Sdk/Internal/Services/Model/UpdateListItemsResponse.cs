using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class UpdateListItemsResponse : ApiResponse
    {
        [JsonProperty("object")]
        public APObject Object { get; set; }
    }
}
