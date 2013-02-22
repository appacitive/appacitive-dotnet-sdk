using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public struct TimeFieldValue : IFieldValue
    {
        public TimeFieldValue(object o)
            : this()
        {
            this.Value = "time('" + ((DateTime)o).ToString(Formats.Time) + "')";
        }

        public string Value { get; private set; }

        public string GetStringValue()
        {
            return this.Value;
        }
    }

}
