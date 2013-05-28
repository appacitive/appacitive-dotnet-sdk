using Appacitive.Sdk.Realtime;
using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public enum TypeClassification
    {
        Schema,
        Relation
    }

    public class ObjectSubscriber
    {
        public ObjectSubscriber(TypeClassification classification, string type, string id)
        {
            this.TypeClassification = classification;
            this.DataType = type;
            this.Id = id;
        }

        public TypeClassification TypeClassification { get; set; }

        public string DataType { get; set; }

        public string Id { get; set; }

        public event Action<RealTimeMessage> Updated
        {
            add 
            {
                var eventType = this.TypeClassification == Internal.TypeClassification.Schema ? EventType.ArticleUpdate : EventType.ConnectionUpdate;
                EventProxy.Add(new ObjectTopic(eventType, this.DataType, this.Id), value); 
            }
            remove 
            {
                var eventType = this.TypeClassification == Internal.TypeClassification.Schema ? EventType.ArticleUpdate : EventType.ConnectionUpdate;
                EventProxy.Remove(new ObjectTopic(eventType, this.DataType, this.Id), value); 
            }
        }

        public event Action<RealTimeMessage> Deleted
        {
            add
            {
                var eventType = this.TypeClassification == Internal.TypeClassification.Schema ? EventType.ArticleDelete : EventType.ConnectionDelete;
                EventProxy.Add(new ObjectTopic(eventType, this.DataType, this.Id), value);
            }
            remove
            {
                var eventType = this.TypeClassification == Internal.TypeClassification.Schema ? EventType.ArticleDelete : EventType.ConnectionDelete;
                EventProxy.Remove(new ObjectTopic(eventType, this.DataType, this.Id), value);
            }
        }
    }
}
