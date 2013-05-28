using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class TypeSubscriber
    {
        public TypeSubscriber(TypeClassification classification, string dataType)
        {
            this.TypeClassification = classification;
            this.DataType = dataType;
        }

        public TypeClassification TypeClassification { get; set; }

        public string DataType { get; set; }

        public event Action<RealTimeMessage> Created
        {
            add
            {
                var eventType = this.TypeClassification == Internal.TypeClassification.Schema ? EventType.ArticleCreate : EventType.ConnectionCreate;
                EventProxy.Add(new TypeTopic(eventType, this.DataType), value);
            }
            remove
            {
                var eventType = this.TypeClassification == Internal.TypeClassification.Schema ? EventType.ArticleCreate : EventType.ConnectionCreate;
                EventProxy.Remove(new TypeTopic(eventType, this.DataType), value);
            }
        }

        public event Action<RealTimeMessage> Updated
        {
            add
            {
                var eventType = this.TypeClassification == Internal.TypeClassification.Schema ? EventType.ArticleUpdate : EventType.ConnectionUpdate;
                EventProxy.Add(new TypeTopic(eventType, this.DataType), value);
            }
            remove
            {
                var eventType = this.TypeClassification == Internal.TypeClassification.Schema ? EventType.ArticleUpdate : EventType.ConnectionUpdate;
                EventProxy.Remove(new TypeTopic(eventType, this.DataType), value);
            }
        }

        public event Action<RealTimeMessage> Deleted
        {
            add
            {
                var eventType = this.TypeClassification == Internal.TypeClassification.Schema ? EventType.ArticleDelete : EventType.ConnectionDelete;
                EventProxy.Add(new TypeTopic(eventType, this.DataType), value);
            }
            remove
            {
                var eventType = this.TypeClassification == Internal.TypeClassification.Schema ? EventType.ArticleDelete : EventType.ConnectionDelete;
                EventProxy.Remove(new TypeTopic(eventType, this.DataType), value);
            }
        }
    }
}
