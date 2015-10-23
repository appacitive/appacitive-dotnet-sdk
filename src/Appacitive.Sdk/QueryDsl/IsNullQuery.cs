using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    internal class IsNullQuery : IQuery
    {
        public IsNullQuery(Field field)
        {
            this.Field = field;
        }

        public override string ToString()
        {
            return this.AsString();
        }

        public string AsString()
        {
            return string.Format("{0} is null", this.Field);    
        }

        public Field Field { get; set; }
    }
}
