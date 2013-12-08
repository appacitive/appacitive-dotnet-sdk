using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public struct DynamicValue
    {
        public DynamicValue(decimal value) : this(value.ToString(), DynamicValueType.Decimal) { }

        public DynamicValue(double value) : this(Convert.ToDecimal(value).ToString(), DynamicValueType.Decimal) { }

        public DynamicValue(DateTime value) : this(value.ToString("o"), DynamicValueType.DateTime) { }

        public DynamicValue(long value) : this(value.ToString(), DynamicValueType.Int64) { }

        public DynamicValue(bool value) : this(value.ToString(), DynamicValueType.Boolean) { }

        public DynamicValue(string value) : this(value, DynamicValueType.String) { }

        private DynamicValue(string value, DynamicValueType typeCode)
            : this()
        {
            StringValue = value;
            TypeCode = typeCode;
        }

        private DynamicValueType TypeCode { get; set; }
        public string StringValue { get; private set; }

        #region Implicit conversions 

        // decimal representation
        public static implicit operator decimal(DynamicValue value)
        {
            return decimal.Parse(value.StringValue);
        }

        public static implicit operator DynamicValue(decimal d)
        {
            return new DynamicValue(d);
        }

        // int rep
        public static implicit operator int(DynamicValue value)
        {
            return int.Parse(value.StringValue);
        }

        public static implicit operator DynamicValue(int value)
        {
            return new DynamicValue(value);
        }

        // long rep
        public static implicit operator long(DynamicValue value)
        {
            return long.Parse(value.StringValue);
        }

        public static implicit operator DynamicValue(long d)
        {
            return new DynamicValue(d);
        }

        // float rep
        public static implicit operator float(DynamicValue value)
        {
            return float.Parse(value.StringValue);
        }

        public static implicit operator DynamicValue(float d)
        {
            return new DynamicValue(d);
        }

        // string rep
        public static implicit operator string(DynamicValue value)
        {
            return value.StringValue;
        }

        public static implicit operator DynamicValue(string s)
        {
            return new DynamicValue(s);
        }

        // double rep
        public static implicit operator double(DynamicValue value)
        {
            return double.Parse(value.StringValue);
        }

        public static implicit operator DynamicValue(double d)
        {
            return new DynamicValue(d);
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

        public static implicit operator bool(DynamicValue value)
        {
            return IsTrue(value.StringValue);
        }

        public static implicit operator DynamicValue(bool d)
        {
            return new DynamicValue(d);
        }

        // date time
        public static implicit operator DateTime(DynamicValue value)
        {
            DateTime date;
            if( DateTime.TryParse(value.StringValue, out date) == false )
                return DateTime.ParseExact(value.StringValue, new[] { "o", Formats.Date, Formats.Time }, null, System.Globalization.DateTimeStyles.None);
            return date;
        }

        public static implicit operator DynamicValue(DateTime d)
        {
            return new DynamicValue(d);
        }

        #endregion

        #region Equality operator

        // decimal equality
        public static bool operator ==(DynamicValue v1, decimal num)
        {
            return decimal.Parse(v1.StringValue) == num;
        }

        public static bool operator ==(decimal num, DynamicValue v)
        {
            return decimal.Parse(v.StringValue) == num;
        }

        public static bool operator !=(DynamicValue v1, decimal num)
        {
            return decimal.Parse(v1.StringValue) != num;
        }

        public static bool operator !=(decimal num, DynamicValue v)
        {
            return decimal.Parse(v.StringValue) != num;
        }

        // float equality
        public static bool operator ==(DynamicValue value, float num)
        {
            return decimal.Parse(value.StringValue) == Convert.ToDecimal(num);
        }

        public static bool operator ==(float num, DynamicValue value)
        {
            return decimal.Parse(value.StringValue) == Convert.ToDecimal(num);
        }

        public static bool operator !=(DynamicValue value, float num)
        {
            return decimal.Parse(value.StringValue) != Convert.ToDecimal(num);
        }

        public static bool operator !=(float num, DynamicValue value)
        {
            return decimal.Parse(value.StringValue) != Convert.ToDecimal(num);
        }

        // double equality
        public static bool operator ==(DynamicValue value, double num)
        {
            return decimal.Parse(value.StringValue) == Convert.ToDecimal(num);
        }

        public static bool operator ==(double num, DynamicValue value)
        {
            return decimal.Parse(value.StringValue) == Convert.ToDecimal(num);
        }

        public static bool operator !=(DynamicValue value, double num)
        {
            return decimal.Parse(value.StringValue) != Convert.ToDecimal(num);
        }

        public static bool operator !=(double num, DynamicValue value)
        {
            return decimal.Parse(value.StringValue) != Convert.ToDecimal(num);
        }

        // long equality
        public static bool operator ==(DynamicValue value, long num)
        {
            return long.Parse(value.StringValue) == num;
        }

        public static bool operator ==(long num, DynamicValue value)
        {
            return long.Parse(value.StringValue) == num;
        }

        public static bool operator !=(DynamicValue value, long num)
        {
            return long.Parse(value.StringValue) != num;
        }

        public static bool operator !=(long num, DynamicValue value)
        {
            return long.Parse(value.StringValue) != num;
        }

        // int equality
        public static bool operator ==(DynamicValue value, int num)
        {
            return long.Parse(value.StringValue) == num;
        }

        public static bool operator ==(int num, DynamicValue value)
        {
            return long.Parse(value.StringValue) == num;
        }

        public static bool operator !=(DynamicValue value, int num)
        {
            return long.Parse(value.StringValue) != num;
        }

        public static bool operator !=(int num, DynamicValue value)
        {
            return long.Parse(value.StringValue) != num;
        }

        // Date time equality
        public static bool operator ==(DynamicValue value, DateTime datetime)
        {
            DateTime date = value;
            return date == datetime;
        }

        public static bool operator ==(DateTime datetime, DynamicValue value)
        {
            DateTime date = value;
            return date == datetime;
        }

        public static bool operator !=(DynamicValue value, DateTime datetime)
        {
            DateTime date = value;
            return date != datetime;
        }

        public static bool operator !=(DateTime datetime, DynamicValue value)
        {
            DateTime date = value;
            return date != datetime;
        }

        // Boolean equality
        public static bool operator ==(DynamicValue value, bool boolean)
        {
            bool bool1 = value;
            return bool1 == boolean;
        }

        public static bool operator ==(bool boolean, DynamicValue value)
        {
            bool bool1 = value;
            return bool1 == boolean;
        }

        public static bool operator !=(DynamicValue value, bool boolean)
        {
            bool bool1 = value;
            return bool1 != boolean;
        }

        public static bool operator !=(bool boolean, DynamicValue value)
        {
            bool bool1 = value;
            return bool1 != boolean;
        }

        // String equality
        public static bool operator ==(DynamicValue value, string str)
        {
            return value.StringValue == str;
        }

        public static bool operator ==(string str, DynamicValue value)
        {
            return value.StringValue == str;
        }

        public static bool operator !=(DynamicValue value, string str)
        {
            return value.StringValue != str;
        }

        public static bool operator !=(string str, DynamicValue value)
        {
            return value.StringValue != str;
        }

        public static bool operator ==(DynamicValue v1, DynamicValue v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(DynamicValue v1, DynamicValue v2)
        {
            return v1.Equals(v2) == false;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null || obj is DynamicValue == false || this.StringValue == null)
                return false;
            var other = (DynamicValue)obj;
            if (this.StringValue == null || other.StringValue == null)
                return false;
            return this.StringValue.Equals(other.StringValue);
        }

        public override int GetHashCode()
        {
            return this.StringValue.GetHashCode();
        }
    }
}
