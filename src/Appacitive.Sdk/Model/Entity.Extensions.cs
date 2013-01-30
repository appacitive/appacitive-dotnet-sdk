using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public partial class Entity
    {
        public long AsInt(string property)
        {
            return int.Parse(this[property]);
        }

        public decimal AsDecimal(string property)
        {
            return decimal.Parse(this[property]);
        }

        public bool AsBool(string property)
        {
            return bool.Parse(this[property]);
        }

        public DateTime AsDateTime(string property)
        {
            return DateTime.ParseExact(this[property], Formats.DateTime, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
        }

        public DateTime AsDate(string property)
        {
            //TODO: Confirm date format string
            var value = this[property];
            return DateTime.ParseExact(value, new [] {Formats.DateTime, Formats.BirthDate }, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
        }
    }
}
