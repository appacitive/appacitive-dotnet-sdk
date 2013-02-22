using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    [Serializable]
    public class FindAllArticleResponse : ApiResponse
    {
        [JsonProperty("articles")]
        public List<Article> Articles { get; set; }

        [JsonProperty("paginginfo")]
        public PagingInfo PagingInfo { get; set; }

        internal static FindAllArticleResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<FindAllArticleResponse>(bytes);
        }
    }

    [Serializable]
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
