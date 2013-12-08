using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Realtime;

namespace Appacitive.Sdk.Services
{
    public class FindAllObjectsResponse : ApiResponse
    {
        [JsonProperty("articles")]
        public List<APObject> Objects { get; set; }

        [JsonProperty("paginginfo")]
        public PagingInfo PagingInfo { get; set; }
    }

}
