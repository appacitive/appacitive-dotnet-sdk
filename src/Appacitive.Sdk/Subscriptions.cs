using Appacitive.Sdk.Realtime;
using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Realtime
{
    public static class Subscriptions
    {
        public static readonly object _syncRoot = new object();

        public static ObjectSubscriber ForObject(string schema, string id)
        {
            return new ObjectSubscriber(TypeClassification.Schema, schema, id);
        }

        public static ObjectSubscriber ForConnection(string relation, string id)
        {
            return new ObjectSubscriber(TypeClassification.Relation, relation, id);
        }

        public static TypeSubscriber ForSchema(string schema)
        {
            return new TypeSubscriber(TypeClassification.Schema, schema);
        }

        public static TypeSubscriber ForRelation(string relation)
        {
            return new TypeSubscriber(TypeClassification.Relation, relation);
        }
    }
}
