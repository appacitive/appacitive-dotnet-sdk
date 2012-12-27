using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class DeleteArticleRequest : ApiRequest
    {
        public string Type { get; set; }

        public string Id { get; set; }

        public override byte[] ToBytes()
        {
            return null;
        }
    }

    public class GetArticleRequest : ApiRequest
    {
        public string Type { get; set; }

        public string Id { get; set; }

        public override byte[] ToBytes()
        {
            return null;
        }
    }

    public class CreateSessionRequest : ApiRequest
    {
        [JsonProperty("apikey")]
        public string ApiKey { get; set; }

        [JsonProperty("isnonsliding")]
        public bool IsNonSliding { get; set; }

        [JsonProperty("windowtime")]
        public int WindowTime { get; set; }

        [JsonProperty("usagecount")]
        public int UsageCount { get; set; }
    }

    public class CreateSessionResponse : ApiResponse
    {
        [JsonProperty("session")]
        public Session Session { get; set; }

        public static CreateSessionResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = AppacitiveContext.ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<CreateSessionResponse>(bytes);
        }
    }

    public class Session
    {
        [JsonProperty("sessionkey")]
        public string SessionKey { get; set; }

        [JsonProperty("usagecount")]
        public long UsageCount { get; set; }

        [JsonProperty("isnonsliding")]
        public bool IsNonSliding { get; set; }

        [JsonProperty("windowtime")]
        public int WindowTime { get; set; }
    }

    public class GetArticleResponse : ApiResponse
    {
        [JsonProperty("article")]
        public Article Article { get; set; }

        public static GetArticleResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<GetArticleResponse>(bytes);
        }
    }

    public class CreateArticleRequest : ApiRequest
    {
        public Article Article { get; set; }

        public override byte[] ToBytes()
        {
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Serialize(this.Article);
        }
    }

    public class CreateArticleResponse : ApiResponse
    {
        [JsonProperty("article")]
        public Article Article { get; set; }

        public static CreateArticleResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<CreateArticleResponse>(bytes);
        }
    }

    public abstract class ApiRequest
    {
        protected ApiRequest()
        {
            this.SessionToken = AppacitiveContext.SessionToken;
            this.CurrentLocation = AppacitiveContext.UserLocation;
            this.Verbosity = AppacitiveContext.Verbosity;
            this.UserToken = AppacitiveContext.UserToken;
            this.Environment = AppacitiveContext.Environment;
        }

        [JsonIgnore]
        public string SessionToken { get; set; }

        [JsonIgnore]
        public string UserToken { get; set; }

        [JsonIgnore]
        public Environment Environment { get; set; }

        [JsonIgnore]
        public bool DebugEnabled { get; set; }

        [JsonIgnore]
        public Geocode CurrentLocation { get; set; }

        [JsonIgnore]
        public Verbosity Verbosity { get; set; }

        public virtual byte[] ToBytes()
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Serialize(this);
        }
    }

    public class UpdateArticleRequest : ApiRequest
    {
        public UpdateArticleRequest()
        {
            this.PropertyUpdates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.AddedTags = new List<string>();
            this.RemovedTags = new List<string>();
        }

        [JsonIgnore]
        public string Id { get; set; }

        [JsonIgnore]
        public string Type { get; set; }

        [JsonIgnore]
        public IDictionary<string, string> PropertyUpdates { get; private set; }

        [JsonIgnore]
        public List<string> AddedTags { get; private set; }

        [JsonIgnore]
        public List<string> RemovedTags { get; private set; }
    }

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

    public abstract class ApiResponse
    {
        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonIgnore]
        public decimal TimeTaken { get; set; }
    }

    public class Status
    {
        public static Status Parse(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<Status>(data);
        }

        [JsonProperty("referenceid")]
        public string ReferenceId { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("faulttype")]
        public string FaultType { get; set; }

        [JsonProperty("additionalmessages")]
        public List<string> AdditionalMessages { get; set; }

        [JsonIgnore]
        public bool IsSuccessful
        {
            get { return this.Code == "200"; }
        }
    }
}
