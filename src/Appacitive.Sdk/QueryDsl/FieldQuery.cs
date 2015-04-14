using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
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
            public static readonly string Match = "match";
        }

        public static FieldQuery IsEqualTo(Field field, string value)
        {
            return new FieldQuery() { Field = field, Value = new PrimtiveFieldValue(value), Operator = Operators.IsEqualTo };
        }

        public static FieldQuery IsEqualTo(Field field, long value)
        {
            return new FieldQuery() { Field = field, Value = new PrimtiveFieldValue(value), Operator = Operators.IsEqualTo };
        }

        public static FieldQuery IsEqualTo(Field field, decimal value)
        {
            return new FieldQuery() { Field = field, Value = new PrimtiveFieldValue(value), Operator = Operators.IsEqualTo };
        }

        public static FieldQuery IsEqualTo(Field field, bool value)
        {
            return new FieldQuery() { Field = field, Value = new PrimtiveFieldValue(value), Operator = Operators.IsEqualTo };
        }

        public static FieldQuery IsEqualTo(Field field, DateTime value)
        {
            return new FieldQuery() { Field = field, Value = new PrimtiveFieldValue(value), Operator = Operators.IsEqualTo };
        }

        public static IQuery IsGreaterThan(Field field, decimal value)
        {
            return new FieldQuery { Field = field, Operator = Operators.IsGreaterThan, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsGreaterThan(Field field, long value)
        {
            return new FieldQuery { Field = field, Operator = Operators.IsGreaterThan, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsGreaterThan(Field field, DateTime value)
        {
            return new FieldQuery { Field = field, Operator = Operators.IsGreaterThan, Value = new PrimtiveFieldValue(value) };
        }


        public static IQuery IsGreaterThanEqualTo(Field field, long value)
        {
            return new FieldQuery { Field = field, Operator = Operators.IsGreaterThanEqualTo, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsGreaterThanEqualTo(Field field, decimal value)
        {
            return new FieldQuery { Field = field, Operator = Operators.IsGreaterThanEqualTo, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsGreaterThanEqualTo(Field field, DateTime value)
        {
            return new FieldQuery { Field = field, Operator = Operators.IsGreaterThanEqualTo, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsLessThan(Field field, long value)
        {
            return new FieldQuery { Field = field, Operator = Operators.IsLessThan, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsLessThan(Field field, decimal value)
        {
            return new FieldQuery { Field = field, Operator = Operators.IsLessThan, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsLessThan(Field field, DateTime value)
        {
            return new FieldQuery { Field = field, Operator = Operators.IsLessThan, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsLessThanEqualTo(Field field, long value)
        {
            return new FieldQuery { Field = field, Operator = Operators.IsLessThanEqualTo, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsLessThanEqualTo(Field field, decimal value)
        {
            return new FieldQuery { Field = field, Operator = Operators.IsLessThanEqualTo, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery IsLessThanEqualTo(Field field, DateTime value)
        {
            return new FieldQuery { Field = field, Operator = Operators.IsLessThanEqualTo, Value = new PrimtiveFieldValue(value) };
        }

        public static IQuery Like(Field field, string value)
        {
            return new FieldQuery { Field = field, Operator = Operators.Like, Value = new PrimtiveFieldValue("*" + value + "*") };
        }

        public static IQuery StartsWith(Field field, string value)
        {
            return new FieldQuery { Field = field, Operator = Operators.Like, Value = new PrimtiveFieldValue(value + "*") };
        }

        public static IQuery EndsWith(Field field, string value)
        {
            return new FieldQuery { Field = field, Operator = Operators.Like, Value = new PrimtiveFieldValue("*" + value) };
        }

        public Field Field { get; set; }

        public string Operator { get; set; }

        public IFieldValue Value { get; set; }

        public override string ToString()
        {
            return this.AsString();   
        }



        public string AsString()
        {
            return string.Format("{0} {1} {2}",
                this.Field.ToString(),
                this.Operator,
                this.Value.GetStringValue());
        }

        internal static IQuery FreeTextMatches(Field field, string freeTextExpression)
        {
            return new FieldQuery { Field = field, Operator = Operators.Match, Value = new PrimtiveFieldValue(freeTextExpression) };
        }
    }
}
