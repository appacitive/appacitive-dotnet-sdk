using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    internal class RadialSearchQuery : IQuery
    {
        public RadialSearchQuery(Field field, Geocode center, decimal radius, DistanceUnit unit = Sdk.DistanceUnit.Miles)
        {
            this.Field = field;
            this.Center = center;
            this.DistanceUnit = unit;
            this.Radius = radius;
        }

        public Field Field { get; set; }

        public Geocode Center { get; set; }

        public decimal Radius { get; set; }

        public DistanceUnit DistanceUnit { get; set; }

        public IFieldValue Value { get; set; }

        public override string ToString()
        {
            return this.AsString();
        }

        public string AsString()
        {
            return string.Format("{0} within_circle {1},{2} {3}",
                this.Field.ToString(),
                this.Center,
                this.Radius,
                this.DistanceUnit == Sdk.DistanceUnit.Miles ? "mi" : "km");
        }
    }
}
