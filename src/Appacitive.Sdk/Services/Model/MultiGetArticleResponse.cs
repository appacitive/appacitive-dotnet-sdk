using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class MultiGetArticleResponse : ApiResponse
    {
        [JsonProperty("articles")]
        public List<Article> Articles { get; set; }

        public static MultiGetArticleResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<MultiGetArticleResponse>(bytes);
        }
    }
}
