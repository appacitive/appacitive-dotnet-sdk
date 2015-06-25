using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class AggregateExpression
    {
        internal AggregateExpression(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }

        public IQuery IsNull()
        {
            return new IsNullQuery(Field.Aggregate(this.Name));
        }
        
        public IQuery IsEqualTo(decimal value)
        {
            return FieldQuery.IsEqualTo(FieldType.Aggregate, this.Name, value);
        }

        public IQuery IsGreaterThan(decimal value)
        {
            return FieldQuery.IsGreaterThan(FieldType.Aggregate, this.Name, value);
        }

        public IQuery IsGreaterThanEqualTo(decimal value)
        {
            return FieldQuery.IsGreaterThanEqualTo(FieldType.Aggregate, this.Name, value);
        }

        public IQuery IsLessThan(decimal value)
        {
            return FieldQuery.IsLessThan(FieldType.Aggregate, this.Name, value);
        }

        public IQuery IsLessThanEqualTo(decimal value)
        {
            return FieldQuery.IsLessThanEqualTo(FieldType.Aggregate, this.Name, value);
        }

        public IQuery Between(decimal before, decimal after)
        {
            return BetweenQuery.Between(FieldType.Aggregate, this.Name, before, after);
        }

        public IQuery Between(long before, long after)
        {
            return BetweenQuery.Between(FieldType.Aggregate, this.Name, before, after);
        }
    }
}
