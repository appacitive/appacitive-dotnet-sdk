using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class AggregateExpression
    {
        internal AggregateExpression(string name)
        {
            this.Field = name;
        }

        public string Field { get; private set; }

        public IQuery IsEqualTo(decimal value)
        {
            return FieldQuery.IsEqualTo(FieldType.Aggregate, this.Field, value);
        }

        public IQuery IsGreaterThan(decimal value)
        {
            return FieldQuery.IsGreaterThan(FieldType.Aggregate, this.Field, value);
        }

        public IQuery IsGreaterThanEqualTo(decimal value)
        {
            return FieldQuery.IsGreaterThanEqualTo(FieldType.Aggregate, this.Field, value);
        }

        public IQuery IsLessThan(decimal value)
        {
            return FieldQuery.IsLessThan(FieldType.Aggregate, this.Field, value);
        }

        public IQuery IsLessThanEqualTo(decimal value)
        {
            return FieldQuery.IsLessThanEqualTo(FieldType.Aggregate, this.Field, value);
        }

        public IQuery Between(decimal before, decimal after)
        {
            return BetweenQuery.Between(FieldType.Aggregate, this.Field, before, after);
        }

        public IQuery Between(long before, long after)
        {
            return BetweenQuery.Between(FieldType.Aggregate, this.Field, before, after);
        }
    }
}
