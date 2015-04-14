using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    internal class BetweenQuery : IQuery
    {
        private BetweenQuery(Field field, IFieldValue greaterThanEqualTo, IFieldValue lessThanEqualTo)
        {
            this.Field = field;
            this.GreaterThanEqualTo = greaterThanEqualTo;
            this.LessThanEqualTo = lessThanEqualTo;
        }

        public static BetweenQuery Between(Field field, decimal greaterThanEqualTo, decimal lessThanEqualTo)
        {
            return new BetweenQuery(field, new PrimtiveFieldValue(greaterThanEqualTo), new PrimtiveFieldValue(lessThanEqualTo));
        }

        public static BetweenQuery Between(Field field, long greaterThanEqualTo, long lessThanEqualTo)
        {
            return new BetweenQuery(field, new PrimtiveFieldValue(greaterThanEqualTo), new PrimtiveFieldValue(lessThanEqualTo));
        }

        public static BetweenQuery Between(Field field, DateTime greaterThanEqualTo, DateTime lessThanEqualTo)
        {
            return new BetweenQuery(field, new PrimtiveFieldValue(greaterThanEqualTo), new PrimtiveFieldValue(lessThanEqualTo));
        }

        public Field Field { get; set; }

        public string Operator { get; set; }

        public IFieldValue GreaterThanEqualTo { get; set; }

        public IFieldValue LessThanEqualTo { get; set; }

        public string AsString()
        {
            //*last_read_timestamp between (datetime(‘2012-04-10:00:00:00.0000000z’),datetime(‘2012-05-10:00:00:00.0000000z’))
            return string.Format("{0} between ({1},{2})",
                this.Field.ToString(),
                this.GreaterThanEqualTo.GetStringValue(),
                this.LessThanEqualTo.GetStringValue());
        }

        

        public override string ToString()
        {
            return this.AsString();
        }
    }

}
