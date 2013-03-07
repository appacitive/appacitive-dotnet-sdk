using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Interfaces;

namespace Appacitive.Sdk.Services
{
    public class UpdateArticleResponse : ApiResponse
    {
        public static UpdateArticleResponse Parse(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<UpdateArticleResponse>(data);
        }

        [JsonProperty("article")]
        public Article Article { get; set; }
    }
}
