using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    internal class BetweenQuery : IQuery
    {
        private BetweenQuery(FieldType type, string name, IFieldValue greaterThanEqualTo, IFieldValue lessThanEqualTo)
        {
        }

        public static BetweenQuery Between(FieldType fieldType, string fieldName, decimal greaterThanEqualTo, decimal lessThanEqualTo)
        {
            return new BetweenQuery(fieldType, fieldName, new PrimtiveFieldValue(greaterThanEqualTo), new PrimtiveFieldValue(lessThanEqualTo));
        }

        public static BetweenQuery Between(FieldType fieldType, string fieldName, long greaterThanEqualTo, long lessThanEqualTo)
        {
            return new BetweenQuery(fieldType, fieldName, new PrimtiveFieldValue(greaterThanEqualTo), new PrimtiveFieldValue(lessThanEqualTo));
        }

        public static BetweenQuery Between(FieldType fieldType, string fieldName, DateTime greaterThanEqualTo, DateTime lessThanEqualTo)
        {
            return new BetweenQuery(fieldType, fieldName, new PrimtiveFieldValue(greaterThanEqualTo), new PrimtiveFieldValue(lessThanEqualTo));
        }

        public string Field { get; set; }

        public FieldType FieldType { get; set; }

        public string Operator { get; set; }

        public IFieldValue GreaterThanEqualTo { get; set; }

        public IFieldValue LessThanEqualTo { get; set; }

        public string AsString()
        {
            //*last_read_timestamp between (datetime(‘2012-04-10:00:00:00.0000000z’),datetime(‘2012-05-10:00:00:00.0000000z’))
            return string.Format("{0}{1} betweeb ({2},{3})",
                this.GetPrefix(),
                this.Field,
                this.GreaterThanEqualTo.GetStringValue(),
                this.LessThanEqualTo.GetStringValue());
        }

        // Move this to extension method
        private string GetPrefix()
        {
            switch (this.FieldType)
            {
                case FieldType.Property:
                    return "*";
                case FieldType.Attribute:
                    return "@";
                case FieldType.Aggregate:
                    return "$";
                default: throw new Exception("Unsuported field type.");
            }
        }

        public override string ToString()
        {
            return this.AsString();
        }
    }

}
