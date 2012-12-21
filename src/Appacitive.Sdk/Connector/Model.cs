using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Connector
{
    public class GetArticleRequest : ApiRequest
    {
        public string Type { get; set; }

        public string Id { get; set; }

        public override byte[] ToBytes()
        {
            return null;
        }
    }

    public class GetArticleResponse : ApiResponse
    {
        public Article Article { get; set; }

        public static GetArticleResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = null;
            return serializer.Deserialize<GetArticleResponse>(bytes);
        }
    }

    public abstract class ApiRequest
    {
        public string SessionToken { get; set; }

        public string UserToken { get; set; }

        public Environment Environment { get; set; }

        public bool DebugEnabled { get; set; }

        public Geocode CurrentLocation { get; set; }

        public Verbosity Verbosity { get; set; }

        public abstract byte[] ToBytes();

    }

    public abstract class ApiResponse
    {
        public Status Status { get; set; }
    }

    public class Status
    {
        public Status()
        {
            this.AdditionalMessages = new List<string>();
        }

        public string ReferenceId { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public string FaultType { get; set; }

        public List<string> AdditionalMessages { get; private set; }

        public bool IsSuccessful
        {
            get { return this.Code == "200"; }
        }
    }
}
