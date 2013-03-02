using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static class Query
    {
        public static readonly string None = string.Empty;

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