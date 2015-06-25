using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public struct PrimtiveFieldValue : IFieldValue
    {
        public PrimtiveFieldValue(object o)
            : this()
        {
            if (o is string)
                this.Value = "'" + o.ToString() + "'";
            else if (o is DateTime)
                this.Value = "datetime('" + ((DateTime)o).ToString(Formats.DateTime) + "')";
            else
                this.Value = o.ToString();
        }
    
        public string Value { get; private set; }

        public string GetStringValue()
        {
            return this.Value;
        }

        public override string ToString()
        {
            return this.GetStringValue();
        }
    }
}
