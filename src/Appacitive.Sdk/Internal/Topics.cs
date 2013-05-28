using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public interface ITopic
    {
        string Key { get; }
    }

    public class ObjectTopic : ITopic
    {
        public ObjectTopic(EventType eventType, string objectType, string id)
        {
            this.EventType = eventType;
            this.ObjectType = objectType.ToLower();
            this.ObjectId = id;
            _key = string.Format("{0}_{1}_{2}", this.EventType.ToString().ToLower(), this.ObjectType, this.ObjectId);
        }

        public EventType EventType { get; private set; }

        public string ObjectType { get; private set; }

        public string ObjectId { get; private set; }

        private string _key;
        public string Key
        {
            get { return _key; }
        }
    }

    public class TypeTopic : ITopic
    {
        public TypeTopic(EventType eventType, string objectType)
        {
            this.EventType = eventType;
            this.ObjectType = objectType.ToLower();
            _key = string.Format("{0}_{1}", this.EventType.ToString().ToLower(), this.ObjectType);
        }

        public EventType EventType { get; private set; }

        public string ObjectType { get; private set; }

        private string _key;
        public string Key
        {
            get { return _key; }
        }
    }

    public class NewMessageTopic : ITopic
    {
        public static readonly ITopic Instance = new NewMessageTopic();

        public string Key
        {
            get { return "new_message"; }
        }
    }

    public class JoinedHubTopic : ITopic
    {
        public static readonly ITopic Instance = new NewMessageTopic();

        public string Key
        {
            get { return "joined_hub"; }
        }
    }

    public class LeftHubTopic : ITopic
    {
        public static readonly ITopic Instance = new NewMessageTopic();

        public string Key
        {
            get { return "left_hub"; }
        }
    }
}
