using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    internal class PolygonSearchQuery : IQuery
    {
        public PolygonSearchQuery(string fieldName, IEnumerable<Geocode> points)
        {
            this.Field = fieldName;
            this.Points = new List<Geocode>(points);
        }

        public string Field { get; set; }

        public IEnumerable<Geocode> Points { get; private set; }

        public override string ToString()
        {
            return this.AsString();   
        }

        private string GetPipeSeparatedList(IEnumerable<Geocode> enumerable)
        {
            var buffer = new StringBuilder();
            enumerable.For(x =>
            {
                if (buffer.Length == 0)
                    buffer.Append(x.ToString());
                else buffer.Append(" | ").Append(x.ToString());
            });
            return buffer.ToString();
        }

        public string AsString()
        {
            return string.Format("*{0} within_polygon {1}", // 0,0 | 10,0 | 0,10",
                this.Field,
                this.GetPipeSeparatedList(this.Points));
        }
    }
}
