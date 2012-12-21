using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public partial class Article : DynamicObject
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
            return DateTime.ParseExact(this[property], "o", null, System.Globalization.DateTimeStyles.AdjustToUniversal);
        }

        public DateTime AsDate(string property)
        {
            //TODO: Confirm date format string
            var value = this[property];
            return DateTime.ParseExact(value, new [] {"o", "yyyy-mm-dd" }, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
        }
    }
}
