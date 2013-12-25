using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static class Query
    {
        public static readonly IQuery None = new RawQuery(null);

        public static IQuery FromRawQuery(string query)
        {
            return new RawQuery(query);
        }

        public static PropertyExpression Property(string name)
        {
            return new PropertyExpression(name);
        }

        public static AttributeExpression Attribute(string name)
        {
            return new AttributeExpression(name);
        }

        public static AggregateExpression Aggregate(string name)
        {
            return new AggregateExpression(name);
        }

        public static TagExpression Tags
        {
            get { return new TagExpression(); }
        }

        public static IQuery And(IEnumerable<IQuery> innerQueries)
        {
            return new AggregateQuery(BoolOperator.And, innerQueries);
        }

        public static IQuery Or(IEnumerable<IQuery> innerQueries)
        {
            return new AggregateQuery(BoolOperator.Or, innerQueries);
        }

        public static IQuery And(params IQuery[] innerQueries)
        {
            return new AggregateQuery(BoolOperator.And, innerQueries);
        }

        public static IQuery Or(params IQuery[] innerQueries)
        {
            return new AggregateQuery(BoolOperator.Or, innerQueries);
        }
    }

    public enum BoolOperator
    {
        And,
        Or
    }

    public enum FieldType
    {
        Property,
        Attribute,
        Aggregate
    }

    public enum DistanceUnit
    {
        Miles,
        Kilometers
    }
}