using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    internal class BetweenQuery : IQuery
    {
        private BetweenQuery(FieldType type, string name, IFieldValue before, IFieldValue after)
        {
        }

        public static BetweenQuery Between(FieldType fieldType, string fieldName, decimal before, decimal after)
        {
            return new BetweenQuery(fieldType, fieldName, new PrimtiveFieldValue(before), new PrimtiveFieldValue(after));
        }

        public static BetweenQuery Between(FieldType fieldType, string fieldName, long before, long after)
        {
            return new BetweenQuery(fieldType, fieldName, new PrimtiveFieldValue(before), new PrimtiveFieldValue(after));
        }

        public static BetweenQuery Between(FieldType fieldType, string fieldName, DateTime before, DateTime after)
        {
            return new BetweenQuery(fieldType, fieldName, new PrimtiveFieldValue(before), new PrimtiveFieldValue(after));
        }

        public static BetweenQuery BetweenDate(FieldType fieldType, string fieldName, DateTime before, DateTime after)
        {
            return new BetweenQuery(fieldType, fieldName, new DateFieldValue(before), new DateFieldValue(after));
        }

        public static BetweenQuery BetweenTime(FieldType fieldType, string fieldName, DateTime before, DateTime after)
        {
            return new BetweenQuery(fieldType, fieldName, new TimeFieldValue(before), new TimeFieldValue(after));
        }

        public string AsString()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return this.AsString();
        }
    }

}
