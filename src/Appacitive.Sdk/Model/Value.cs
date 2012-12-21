using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public struct Value
    {
        public Value(decimal value) : this(value.ToString(), TypeCode.Decimal) { }

        public Value(double value) : this(Convert.ToDecimal(value).ToString(), TypeCode.Decimal) { }

        public Value(DateTime value) : this(value.ToString("o"), TypeCode.DateTime) { }

        public Value(long value) : this(value.ToString(), TypeCode.Int64) { }

        public Value(bool value) : this(value.ToString(), TypeCode.Boolean) { }

        public Value(string value) : this(value, TypeCode.String) { }

        private Value(string value, TypeCode typeCode) : this()
        {
            StringValue = value;
            TypeCode = typeCode;
        }

        private TypeCode TypeCode { get; set; }
        public string StringValue { get; private set; }

        // decimal representation
        public static implicit operator decimal(Value value)
        {
            return decimal.Parse(value.StringValue);
        }

        public static implicit operator Value(decimal d)
        {
            return new Value(d);
        }

        // int rep
        public static implicit operator int(Value value)
        {
            return int.Parse(value.StringValue);
        }

        public static implicit operator Value(int value)
        {
            return new Value(value);
        }

        // long rep
        public static implicit operator long(Value value)
        {
            return long.Parse(value.StringValue);
        }

        public static implicit operator Value(long d)
        {
            return new Value(d);
        }

        // float rep
        public static implicit operator float(Value value)
        {
            return float.Parse(value.StringValue);
        }

        public static implicit operator Value(float d)
        {
            return new Value(d);
        }


        // bool rep
        private static bool IsTrue(string value)
        {
            if ("|Y|Yes|1|true|on|".IndexOf(value, StringComparison.OrdinalIgnoreCase) > -1)
                return true;
            else if ("|N|No|0|false|off|".IndexOf(value, StringComparison.OrdinalIgnoreCase) > -1)
                return false;
            else throw new Exception(string.Format("Cannot convert {0} to boolean.", value));
        }

        public static implicit operator bool(Value value)
        {
            return IsTrue(value.StringValue);
        }

        public static implicit operator Value(bool d)
        {
            return new Value(d);
        }

        // date time
        public static implicit operator DateTime(Value value)
        {
            return DateTime.ParseExact(value.StringValue, "o", null, System.Globalization.DateTimeStyles.AdjustToUniversal);
        }

        public static implicit operator Value(DateTime d)
        {
            return new Value(d);
        }

        public static Value operator +(Value a, decimal b)
        {
            decimal num = a;
            return new Value(num + b);
        }

        public static Value operator +(Value a, long b)
        {
            long num = a;
            return new Value(num + b);
        }

        public static Value operator +(Value a, double b)
        {
            decimal num = a;
            return new Value(num + Convert.ToDecimal(b));
        }

        public static Value operator +(Value a, string str)
        {   
            return new Value(a.StringValue + str);
        }

        // ++ operator
        public static Value operator ++(Value v)
        {
            return ((decimal)v) + 1;
        }

        // -- operator
        public static Value operator --(Value v)
        {
            return ((decimal)v) - 1;
        }

        // decimal equality
        public static bool operator ==(Value v1, decimal num)
        {
            return decimal.Parse(v1.StringValue) == num;
        }

        public static bool operator ==(decimal num, Value v)
        {
            return decimal.Parse(v.StringValue) == num;
        }

        public static bool operator !=(Value v1, decimal num)
        {
            return decimal.Parse(v1.StringValue) != num;
        }

        public static bool operator !=(decimal num, Value v)
        {
            return decimal.Parse(v.StringValue) != num;
        }

        // float equality
        public static bool operator ==(Value value, float num)
        {
            return decimal.Parse(value.StringValue) == Convert.ToDecimal(num);
        }

        public static bool operator ==(float num, Value value)
        {
            return decimal.Parse(value.StringValue) == Convert.ToDecimal(num);
        }

        public static bool operator !=(Value value, float num)
        {
            return decimal.Parse(value.StringValue) != Convert.ToDecimal(num);
        }

        public static bool operator !=(float num, Value value)
        {
            return decimal.Parse(value.StringValue) != Convert.ToDecimal(num);
        }

        // double equality
        public static bool operator ==(Value value, double num)
        {
            return decimal.Parse(value.StringValue) == Convert.ToDecimal(num);
        }

        public static bool operator ==(double num, Value value)
        {
            return decimal.Parse(value.StringValue) == Convert.ToDecimal(num);
        }

        public static bool operator !=(Value value, double num)
        {
            return decimal.Parse(value.StringValue) != Convert.ToDecimal(num);
        }

        public static bool operator !=(double num, Value value)
        {
            return decimal.Parse(value.StringValue) != Convert.ToDecimal(num);
        }

        // long equality
        public static bool operator ==(Value value, long num)
        {
            return long.Parse(value.StringValue) == num;
        }

        public static bool operator ==(long num, Value value)
        {
            return long.Parse(value.StringValue) == num;
        }

        public static bool operator !=(Value value, long num)
        {
            return long.Parse(value.StringValue) != num;
        }

        public static bool operator !=(long num, Value value)
        {
            return long.Parse(value.StringValue) != num;
        }

        // int equality
        public static bool operator ==(Value value, int num)
        {
            return long.Parse(value.StringValue) == num;
        }

        public static bool operator ==(int num, Value value)
        {
            return long.Parse(value.StringValue) == num;
        }

        public static bool operator !=(Value value, int num)
        {
            return long.Parse(value.StringValue) != num;
        }

        public static bool operator !=(int num, Value value)
        {
            return long.Parse(value.StringValue) != num;
        }

        // Date time equality
        public static bool operator ==(Value value, DateTime datetime)
        {
            DateTime date = value;
            return date == datetime;
        }

        public static bool operator ==(DateTime datetime, Value value)
        {
            DateTime date = value;
            return date == datetime;
        }

        public static bool operator !=(Value value, DateTime datetime)
        {
            DateTime date = value;
            return date != datetime;
        }

        public static bool operator !=(DateTime datetime, Value value)
        {
            DateTime date = value;
            return date != datetime;
        }

        // Boolean equality
        public static bool operator ==(Value value, bool boolean)
        {
            bool bool1 = value;
            return bool1 == boolean;
        }

        public static bool operator ==(bool boolean, Value value)
        {
            bool bool1 = value;
            return bool1 == boolean;
        }

        public static bool operator !=(Value value, bool boolean)
        {
            bool bool1 = value;
            return bool1 != boolean;
        }

        public static bool operator !=(bool boolean, Value value)
        {
            bool bool1 = value;
            return bool1 != boolean;
        }

        // String equality
        public static bool operator ==(Value value, string str)
        {
            return value.StringValue == str;
        }

        public static bool operator ==(string str, Value value)
        {
            return value.StringValue == str;
        }

        public static bool operator !=(Value value, string str)
        {
            return value.StringValue != str;
        }

        public static bool operator !=(string str, Value value)
        {
            return value.StringValue != str;
        }

        public static bool operator ==(Value v1, Value v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Value v1, Value v2)
        {
            return v1.Equals(v2) == false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is Value == false || this.StringValue == null)
                return false;
            var other = (Value)obj;
            // Match as number
            if (this.TypeCode == TypeCode.Decimal || this.TypeCode == TypeCode.Int64 ||
                other.TypeCode == TypeCode.Decimal || other.TypeCode == TypeCode.Int64)
                return decimal.Parse(this.StringValue).Equals(decimal.Parse(other.StringValue));

            // Match as bool
            else if (this.TypeCode == TypeCode.Boolean || other.TypeCode == TypeCode.Boolean)
                return bool.Parse(this.StringValue).Equals(bool.Parse(other.StringValue));

            // Match as date
            else if (this.TypeCode == TypeCode.DateTime || other.TypeCode == TypeCode.DateTime)
                return DateTime.ParseExact(this.StringValue, "o", null).Equals(DateTime.ParseExact(other.StringValue, "o", null));

            // Compare as string
            else 
                return this.StringValue.Equals(other.StringValue);
        }

        public override int GetHashCode()
        {
            return this.StringValue.GetHashCode();
        }
    }
}
