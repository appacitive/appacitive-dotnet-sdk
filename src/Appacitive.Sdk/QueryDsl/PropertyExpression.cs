using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class PropertyExpression
    {
        internal PropertyExpression(string name)
        {
            this.Field = Field.Property(name);
        }

        public Field Field { get; private set; }

        
        public IQuery IsNull()
        {
            return new IsNullQuery(this.Field);
        }

        public IQuery IsNotNull()
        {
            return new IsNotNullQuery(this.Field);
        }

        public IQuery IsIn(IEnumerable<string> values)
        {
            return new InQuery(this.Field, values);
        }

        public IQuery IsIn(IEnumerable<long> values)
        {
            return new InQuery(this.Field, values);
        }

        public IQuery IsNotIn(IEnumerable<string> values)
        {
            return new NotInQuery(this.Field, values);
        }

        public IQuery IsNotIn(IEnumerable<long> values)
        {
            return new NotInQuery(this.Field, values);
        }

        public IQuery IsEqualTo(string value)
        {
            // Hack!!! __id mapping to be fixed inside API.
            if( this.Field.Name.Equals("__id", StringComparison.OrdinalIgnoreCase) == true )
                return FieldQuery.IsEqualTo(this.Field, long.Parse(value));
            return FieldQuery.IsEqualTo(this.Field,  StringUtils.EscapeSingleQuotes(value));
        }

        public IQuery IsEqualTo(bool value)
        {
            return FieldQuery.IsEqualTo(this.Field, value);
        }

        public IQuery IsEqualTo(long value)
        {
            return FieldQuery.IsEqualTo(this.Field, value);
        }

        public IQuery IsEqualTo(decimal value)
        {
            return FieldQuery.IsEqualTo(this.Field, value);
        }

        public IQuery IsEqualTo(DateTime value)
        {
            return FieldQuery.IsEqualTo(this.Field, value);
        }

        public IQuery IsGreaterThan(long value)
        {
            return FieldQuery.IsGreaterThan(this.Field, value);
        }

        public IQuery IsGreaterThan(decimal value)
        {
            return FieldQuery.IsGreaterThan(this.Field, value);
        }

        public IQuery IsGreaterThan(DateTime value)
        {
            return FieldQuery.IsGreaterThan(this.Field, value);
        }

        public IQuery IsGreaterThanEqualTo(long value)
        {
            return FieldQuery.IsGreaterThanEqualTo(this.Field, value);
        }

        public IQuery IsGreaterThanEqualTo(decimal value)
        {
            return FieldQuery.IsGreaterThanEqualTo(this.Field, value);
        }

        public IQuery IsGreaterThanEqualTo(DateTime value)
        {
            return FieldQuery.IsGreaterThanEqualTo(this.Field, value);
        }

        public IQuery IsLessThan(long value)
        {
            return FieldQuery.IsLessThan(this.Field, value);
        }

        public IQuery FreeTextMatches(string freeTextExpression)
        {
            return FieldQuery.FreeTextMatches(this.Field, StringUtils.EscapeSingleQuotes(freeTextExpression));
        }



        public IQuery IsLessThan(decimal value)
        {
            return FieldQuery.IsLessThan(this.Field, value);
        }

        public IQuery IsLessThan(DateTime value)
        {
            return FieldQuery.IsLessThan(this.Field, value);
        }

        public IQuery IsLessThanEqualTo(long value)
        {
            return FieldQuery.IsLessThanEqualTo(this.Field, value);
        }

        public IQuery IsLessThanEqualTo(decimal value)
        {
            return FieldQuery.IsLessThanEqualTo(this.Field, value);
        }

        public IQuery IsLessThanEqualTo(DateTime value)
        {
            return FieldQuery.IsLessThanEqualTo(this.Field, value);
        }

        public IQuery Like(string value)
        {
            return FieldQuery.Like(this.Field, StringUtils.EscapeSingleQuotes(value));
        }

        public IQuery StartsWith(string value)
        {
            return FieldQuery.StartsWith(this.Field, StringUtils.EscapeSingleQuotes(value));
        }

        public IQuery EndsWith(string value)
        {
            return FieldQuery.EndsWith(this.Field, StringUtils.EscapeSingleQuotes(value));
        }

        public IQuery WithinCircle(Geocode center, decimal radius, DistanceUnit unit = DistanceUnit.Miles)
        {
            return new RadialSearchQuery(this.Field, center, radius, unit);
        }

        public IQuery WithinPolygon(IEnumerable<Geocode> points)
        {
            return new PolygonSearchQuery(this.Field, points);
        }

        public IQuery WithinPolygon(params Geocode[] points)
        {
            return new PolygonSearchQuery(this.Field, points);
        }

        public IQuery Between(decimal before, decimal after)
        {
            return BetweenQuery.Between(this.Field, before, after);
        }

        public IQuery Between(long before, long after)
        {
            return BetweenQuery.Between(this.Field, before, after);
        }

        public IQuery Between(DateTime before, DateTime after)
        {
            return BetweenQuery.Between(this.Field, before, after);
        }
    }
}
