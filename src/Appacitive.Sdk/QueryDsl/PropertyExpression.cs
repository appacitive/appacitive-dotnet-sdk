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
            this.Name = name;
        }

        public string Name { get; private set; }

        
        public IQuery IsNull()
        {
            return new IsNullQuery(Field.Property(this.Name));
        }

        public IQuery IsIn(IEnumerable<string> values)
        {
            return new InQuery(Field.Property(this.Name), values);
        }

        public IQuery IsIn(IEnumerable<long> values)
        {
            return new InQuery(Field.Property(this.Name), values);
        }

        public IQuery IsEqualTo(string value)
        {
            // Hack!!! __id mapping to be fixed inside API.
            if( this.Name.Equals("__id", StringComparison.OrdinalIgnoreCase) == true )
                return FieldQuery.IsEqualTo(FieldType.Property, this.Name, long.Parse(value));
            return FieldQuery.IsEqualTo(FieldType.Property, this.Name,  StringUtils.EscapeSingleQuotes(value));
        }

        public IQuery IsEqualTo(bool value)
        {
            return FieldQuery.IsEqualTo(FieldType.Property, this.Name, value);
        }

        public IQuery IsEqualTo(long value)
        {
            return FieldQuery.IsEqualTo(FieldType.Property, this.Name, value);
        }

        public IQuery IsEqualTo(decimal value)
        {
            return FieldQuery.IsEqualTo(FieldType.Property, this.Name, value);
        }

        public IQuery IsEqualTo(DateTime value)
        {
            return FieldQuery.IsEqualTo(FieldType.Property, this.Name, value);
        }

        public IQuery IsGreaterThan(long value)
        {
            return FieldQuery.IsGreaterThan(FieldType.Property, this.Name, value);
        }

        public IQuery IsGreaterThan(decimal value)
        {
            return FieldQuery.IsGreaterThan(FieldType.Property, this.Name, value);
        }

        public IQuery IsGreaterThan(DateTime value)
        {
            return FieldQuery.IsGreaterThan(FieldType.Property, this.Name, value);
        }

        public IQuery IsGreaterThanEqualTo(long value)
        {
            return FieldQuery.IsGreaterThanEqualTo(FieldType.Property, this.Name, value);
        }

        public IQuery IsGreaterThanEqualTo(decimal value)
        {
            return FieldQuery.IsGreaterThanEqualTo(FieldType.Property, this.Name, value);
        }

        public IQuery IsGreaterThanEqualTo(DateTime value)
        {
            return FieldQuery.IsGreaterThanEqualTo(FieldType.Property, this.Name, value);
        }

        public IQuery IsLessThan(long value)
        {   
            return FieldQuery.IsLessThan(FieldType.Property, this.Name, value);
        }

        public IQuery FreeTextMatches(string freeTextExpression)
        {
            return FieldQuery.FreeTextMatches(FieldType.Property, this.Name, StringUtils.EscapeSingleQuotes(freeTextExpression));
        }



        public IQuery IsLessThan(decimal value)
        {
            return FieldQuery.IsLessThan(FieldType.Property, this.Name, value);
        }

        public IQuery IsLessThan(DateTime value)
        {
            return FieldQuery.IsLessThan(FieldType.Property, this.Name, value);
        }

        public IQuery IsLessThanEqualTo(long value)
        {
            return FieldQuery.IsLessThanEqualTo(FieldType.Property, this.Name, value);
        }

        public IQuery IsLessThanEqualTo(decimal value)
        {
            return FieldQuery.IsLessThanEqualTo(FieldType.Property, this.Name, value);
        }

        public IQuery IsLessThanEqualTo(DateTime value)
        {
            return FieldQuery.IsLessThanEqualTo(FieldType.Property, this.Name, value);
        }

        public IQuery Like(string value)
        {
            return FieldQuery.Like(FieldType.Property, this.Name, StringUtils.EscapeSingleQuotes(value));
        }

        public IQuery StartsWith(string value)
        {
            return FieldQuery.StartsWith(FieldType.Property, this.Name, StringUtils.EscapeSingleQuotes(value));
        }

        public IQuery EndsWith(string value)
        {
            return FieldQuery.EndsWith(FieldType.Property, this.Name, StringUtils.EscapeSingleQuotes(value));
        }

        public IQuery WithinCircle(Geocode center, decimal radius, DistanceUnit unit = DistanceUnit.Miles)
        {
            return new RadialSearchQuery(this.Name, center, radius, unit);
        }

        public IQuery WithinPolygon(IEnumerable<Geocode> points)
        {
            return new PolygonSearchQuery(this.Name, points);
        }

        public IQuery WithinPolygon(params Geocode[] points)
        {
            return new PolygonSearchQuery(this.Name, points);
        }

        public IQuery Between(decimal before, decimal after)
        {
            return BetweenQuery.Between(FieldType.Property, this.Name, before, after);
        }

        public IQuery Between(long before, long after)
        {
            return BetweenQuery.Between(FieldType.Property, this.Name, before, after);
        }

        public IQuery Between(DateTime before, DateTime after)
        {
            return BetweenQuery.Between(FieldType.Property, this.Name, before, after);
        }
    }
}
