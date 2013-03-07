using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class PagingInfo
    {
        [JsonProperty("pagenumber")]
        public int PageNumber { get; set; }

        [JsonProperty("pagesize")]
        public int PageSize { get; set; }

        [JsonProperty("totalrecords")]
        public int TotalRecords { get; set; }
    }
}
