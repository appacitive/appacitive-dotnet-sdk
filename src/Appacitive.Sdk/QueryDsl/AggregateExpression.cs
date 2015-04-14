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
            this.Field = Field.Aggregate(name);
        }

        public Field Field { get; private set; }

        public IQuery IsNull()
        {
            return new IsNullQuery(this.Field);
        }
        
        public IQuery IsEqualTo(decimal value)
        {
            return FieldQuery.IsEqualTo( this.Field, value);
        }

        public IQuery IsGreaterThan(decimal value)
        {
            return FieldQuery.IsGreaterThan(this.Field, value);
        }

        public IQuery IsGreaterThanEqualTo(decimal value)
        {
            return FieldQuery.IsGreaterThanEqualTo(this.Field, value);
        }

        public IQuery IsLessThan(decimal value)
        {
            return FieldQuery.IsLessThan(this.Field, value);
        }

        public IQuery IsLessThanEqualTo(decimal value)
        {
            return FieldQuery.IsLessThanEqualTo(this.Field, value);
        }

        public IQuery Between(decimal before, decimal after)
        {
            return BetweenQuery.Between(this.Field, before, after);
        }

        public IQuery Between(long before, long after)
        {
            return BetweenQuery.Between(this.Field, before, after);
        }
    }
}
