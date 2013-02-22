using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    internal class FieldQuery : IQuery
    {
        internal static class Operators
        {
            public static readonly string IsEqualTo = "==";
            public static readonly string IsGreaterThan = ">";
            public static readonly string IsGreaterThanEqualTo = ">=";
            public static readonly string IsLessThan = "<";
            public static readonly string IsLessThanEqualTo = "<=";
            public static readonly string Like = "like";
        }

        public static FieldQuery IsEqualTo(FieldType type, string name, string value)
        {
            return new FieldQuery() { Field = name, FieldType = type, Value = new PrimtiveFieldValue(value), Operator = Operators.IsEqualTo };
        }

        public static FieldQuery IsEqualTo(FieldType type, string name, long value)
        {
            return new FieldQuery() { Field = name, FieldType = type, Value = new PrimtiveFieldValue(value), Operator = Operators.IsEqualTo };
        }

        public static FieldQuery IsEqualTo(FieldType type, string name, decimal value)
        {
            return new FieldQuery() { Field = name, FieldType = type, Value = new PrimtiveFieldValue(value), Operator = Operators.IsEqualTo };
        }

        public static FieldQuery IsEqualTo(FieldType type, string name, bool value)
        {
            return new FieldQuery() { Field = name, FieldType = type, Value = new PrimtiveFieldValue(value), Operator = Operators.IsEqualTo };
        }

        public static FieldQuery IsEqualTo(FieldType type, string name, DateTime value)
        {
            return new FieldQuery() { Field = name, FieldType = type, Value = new PrimtiveFieldValue(value), Operator = Operators.IsEqualTo };
        }

        public static FieldQuery IsEqualToDate(FieldType type, string name, DateTime value)
        {
            return new FieldQuery() { Field = name, FieldType = type, Value = new DateFieldValue(value), Operator = Operators.IsEqualTo };
        }

        public static FieldQuery IsEqualToTime(FieldType type, string name, DateTime value)
        {
            return new FieldQuery() { Field = name, FieldType = type, Value = new TimeFieldValue(value), Operator = Operators.IsEqualTo };
        }

        public static IQuery IsGreaterThan(FieldType type, string name, decimal value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsGreaterThan, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsGreaterThan(FieldType type, string name, long value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsGreaterThan, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsGreaterThan(FieldType type, string name, DateTime value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsGreaterThan, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsGreaterThanDate(FieldType type, string name, DateTime value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsGreaterThan, Value = new DateFieldValue(value) };
        }

        public static IQuery IsGreaterThanTime(FieldType type, string name, DateTime value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsGreaterThan, Value = new TimeFieldValue(value) };
        }

        public static IQuery IsGreaterThanEqualTo(FieldType type, string name, long value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsGreaterThanEqualTo, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsGreaterThanEqualTo(FieldType type, string name, decimal value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsGreaterThanEqualTo, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsGreaterThanEqualTo(FieldType type, string name, DateTime value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsGreaterThanEqualTo, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsGreaterThanEqualToDate(FieldType type, string name, DateTime value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsGreaterThanEqualTo, Value = new DateFieldValue(value) };
        }

        public static IQuery IsGreaterThanEqualToTime(FieldType type, string name, DateTime value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsGreaterThanEqualTo, Value = new TimeFieldValue(value) };
        }

        public static IQuery IsLessThan(FieldType type, string name, long value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsLessThan, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsLessThan(FieldType type, string name, decimal value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsLessThan, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsLessThan(FieldType type, string name, DateTime value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsLessThan, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsLessThanDate(FieldType type, string name, DateTime value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsLessThan, Value = new DateFieldValue(value) };
        }

        public static IQuery IsLessThanTime(FieldType type, string name, DateTime value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsLessThan, Value = new TimeFieldValue(value) };
        }

        public static IQuery IsLessThanEqualTo(FieldType type, string name, long value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsLessThanEqualTo, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsLessThanEqualTo(FieldType type, string name, decimal value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsLessThanEqualTo, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsLessThanEqualTo(FieldType type, string name, DateTime value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsLessThanEqualTo, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsLessThanEqualToDate(FieldType type, string name, DateTime value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsLessThanEqualTo, Value = new DateFieldValue(value) };
        }

        public static IQuery IsLessThanEqualToTime(FieldType type, string name, DateTime value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.IsLessThanEqualTo, Value = new TimeFieldValue(value) };
        }

        public static IQuery Like(FieldType type, string name, string value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.Like, Value = new PrimtiveFieldValue("*" + value + "*") };
        }

        public static IQuery StartsWith(FieldType type, string name, string value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.Like, Value = new PrimtiveFieldValue("*" + value) };
        }

        public static IQuery EndsWith(FieldType type, string name, string value)
        {
            return new FieldQuery { FieldType = type, Field = name, Operator = Operators.Like, Value = new PrimtiveFieldValue(value + "*") };
        }


        public string Field { get; set; }

        public FieldType FieldType { get; set; }

        public string Operator { get; set; }

        public IFieldValue Value { get; set; }

        public override string ToString()
        {
            return this.AsString();   
        }



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

        public string AsString()
        {
            return string.Format("{0}{1} {2} {3}",
                this.GetPrefix(),
                this.Field.ToLower(),
                this.Operator,
                this.Value.GetStringValue());
        }
    }
}
