using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Interfaces;

namespace Appacitive.Sdk.Services
{
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

}
