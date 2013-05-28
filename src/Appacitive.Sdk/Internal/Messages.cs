using Appacitive.Sdk.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Realtime
{
    public enum EventType
    {
        ArticleCreate,
        ArticleUpdate,
        ArticleDelete,
        ConnectionCreate,
        ConnectionUpdate,
        ConnectionDelete,
    }

    public class TypeUpdatedMessage : RealTimeMessage, IDownstreamMessage
    {
        public TypeUpdatedMessage()
            : base("7")
        {
        }

        [JsonProperty("et")]
        public EventType EventType { get; set; }

        [JsonProperty("oi")]
        public string ObjectId { get; set; }

        [JsonProperty("ot")]
        public string ObjectType { get; set; }

        public IEnumerable<ITopic> GetTopics()
        {
            yield return new TypeTopic(this.EventType, this.ObjectType);
        }
    }

    public class ObjectUpdatedMessage : RealTimeMessage, IDownstreamMessage
    {
        public ObjectUpdatedMessage()
            : base("8")
        {
        }

        [JsonProperty("et")]
        [JsonConverter(typeof(EventTypeConverter))]
        public EventType EventType { get; set; }

        [JsonProperty("oi")]
        public string ObjectId { get; set; }

        [JsonProperty("ot")]
        public string ObjectType { get; set; }

        public IEnumerable<ITopic> GetTopics()
        {
            yield return new ObjectTopic(this.EventType, this.ObjectType, this.ObjectId);
        }
    }

    public class NewNotificationMessage : RealTimeMessage, IDownstreamMessage
    {
        public NewNotificationMessage()
            : base("9")
        {
        }

        [JsonProperty("u")]
        public string Sender { get; set; }

        [JsonProperty("p")]
        [JsonConverter(typeof(JsonObjectConverter))]
        public IJsonObject Payload { get; set; }

        public IEnumerable<ITopic> GetTopics()
        {
            yield return NewMessageTopic.Instance;
        }
    }

    public class LeftHubMessage : RealTimeMessage, IDownstreamMessage
    {
        public LeftHubMessage()
            : base("11")
        {
        }

        [JsonProperty("u")]
        public string User { get; set; }

        [JsonProperty("h")]
        public string Hub { get; set; }

        public IEnumerable<ITopic> GetTopics()
        {
            yield return LeftHubTopic.Instance;
        }
    }

    public class JoinedHubMessage : RealTimeMessage, IDownstreamMessage
    {
        public JoinedHubMessage()
            : base("10")
        {
        }

        [JsonProperty("u")]
        public string User { get; set; }

        [JsonProperty("h")]
        public string Hub { get; set; }

        public IEnumerable<ITopic> GetTopics()
        {
            yield return JoinedHubTopic.Instance;
        }
    }

    // Upstream messages
    public class UnsubscribeFromObjectChangesMessage : RealTimeMessage
    {
        public UnsubscribeFromObjectChangesMessage()
            : base("6")
        {
        }

        [JsonProperty("et")]
        public string EventType { get; set; }

        [JsonProperty("ot")]
        public string ObjectType { get; set; }

        [JsonProperty("oi")]
        public string Id { get; set; }
    }

    public class SubscribeToObjectChangesMessage : RealTimeMessage
    {
        public SubscribeToObjectChangesMessage()
            : base("5")
        {
        }

        [JsonProperty("et")]
        public string EventType { get; set; }

        [JsonProperty("ot")]
        public string ObjectType { get; set; }

        [JsonProperty("oi")]
        public string Id { get; set; }
    }

    public class SubscribeToHubMessage : RealTimeMessage
    {
        public SubscribeToHubMessage()
            : base("3")
        {
        }

        [JsonProperty("h")]
        public string Hub { get; set; }
    }

    public class UnsubscribeFromHubMessage : RealTimeMessage
    {
        public UnsubscribeFromHubMessage()
            : base("4")
        {
        }

        [JsonProperty("h")]
        public string Hub { get; set; }
    }

    public class SendToHub : RealTimeMessage
    {
        public SendToHub()
            : base("2")
        {
        }

        [JsonProperty("h")]
        public string Hub { get; set; }

        [JsonProperty("p")]
        public JObject Payload { get; set; }

    }

    public class SendToUsers : RealTimeMessage
    {
        public SendToUsers()
            : base("1")
        {
        }

        [JsonProperty("u")]
        public string[] Users { get; set; }

        [JsonProperty("p")]
        public JObject Payload { get; set; }
    }
}
