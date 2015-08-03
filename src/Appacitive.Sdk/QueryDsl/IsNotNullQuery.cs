using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    internal class IsNotNullQuery : IQuery
    {
        public IsNotNullQuery(Field field)
        {
            this.Field = field;
        }

        public string AsString()
        {
            return string.Format("{0} is not null", this.Field);    
        }

        public Field Field { get; set; }
    }
}
