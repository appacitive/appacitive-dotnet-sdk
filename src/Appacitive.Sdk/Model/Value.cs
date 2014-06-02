using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections;
using Appacitive.Sdk.Internal;

namespace Appacitive.Sdk
{
    public abstract class Value
    {
        public static Value FromObject(object obj)
        {
            if (obj == null)
                return NullValue.Instance;
            if (obj.IsMultiValued() == true)
                return new MultiValue(obj as IEnumerable);
            if (SingleValue.IsAllowedValue(obj) == true) 
                return new SingleValue(obj);
            throw new Exception(obj.GetType().Name + " cannot be converted to a Value object.");
        }

        public abstract ValueType Type { get; }

        public abstract T GetValue<T>();

        public abstract IEnumerable<T> GetValues<T>();

        public static implicit operator Value(bool value)
        {
            return new SingleValue(value.ToString());
        }

        public static implicit operator bool(Value value)
        {
            return value.GetValue<bool>();
        }

        public static implicit operator Value(string value)
        {
            if (value == null)
                return null;
            return new SingleValue(value);
        }

        public static implicit operator string(Value value)
        {
            return value.GetValue<string>();
        }


        public static implicit operator Value(int value)
        {
            return new SingleValue(value.ToString());
        }

        public static implicit operator int(Value value)
        {
            return value.GetValue<int>();
        }

        public static implicit operator Value(long value)
        {
            return new SingleValue(value.ToString());
        }

        public static implicit operator long(Value value)
        {
            return value.GetValue<long>();
        }

        public static implicit operator Value(uint value)
        {
            return new SingleValue(value.ToString());
        }

        public static implicit operator uint(Value value)
        {
            return value.GetValue<uint>();
        }

        public static implicit operator Value(ulong value)
        {
            return new SingleValue(value.ToString());
        }

        public static implicit operator ulong(Value value)
        {
            return value.GetValue<ulong>();
        }

        public static implicit operator Value(DateTime value)
        {
            return new SingleValue(value);
        }

        public static implicit operator DateTime(Value value)
        {
            return value.GetValue<DateTime>();
        }

        public static implicit operator Value(float value)
        {
            return new SingleValue(value.ToString());
        }

        public static implicit operator float(Value value)
        {
            return value.GetValue<float>();
        }

        public static implicit operator Value(double value)
        {
            return new SingleValue(value.ToString());
        }

        public static implicit operator double(Value value)
        {
            return value.GetValue<double>();
        }

        public static implicit operator Value(decimal value)
        {
            return new SingleValue(value.ToString());
        }

        public static implicit operator decimal(Value value)
        {
            return value.GetValue<decimal>();
        }

        public static implicit operator Value(Int16 value)
        {
            return new SingleValue(value.ToString());
        }

        public static implicit operator Int16(Value value)
        {
            return value.GetValue<Int16>();
        }

        public static implicit operator Value(UInt16  value)
        {
            return new SingleValue(value.ToString());
        }

        public static implicit operator UInt16(Value value)
        {
            return value.GetValue<UInt16>();
        }

        public static implicit operator Value(char value)
        {
            return new SingleValue(value.ToString());
        }

        public static implicit operator char(Value value)
        {
            return value.GetValue<char>();
        }

        public override string ToString()
        {
            return this.GetValue<string>();
        }
    }

    public enum ValueType
    {
        Null,
        SingleValue,
        MultiValue
    }

    public class NullValue : Value
    {
        private NullValue()
        {
        }

        public static Value Instance = new NullValue();

        public override ValueType Type
        {
            get { return ValueType.Null; }
        }

        public override T GetValue<T>()
        {
            if (default(T) == null)
                return default(T);
            else throw new Exception(typeof(T).Name + " is not nullable.");
        }

        public override IEnumerable<T> GetValues<T>()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return this == obj;
        }

        public override int GetHashCode()
        {
            // Dont need to override since there will always be only once instance of this class
            // since it is a singleton.
            return base.GetHashCode();   
        }
    }

    public class SingleValue : Value
    {
        public SingleValue(object value)
        {
            if( SingleValue.IsAllowedValue(value) == false )
                throw new ArgumentException("value must by a geocode, string, datetime or primitive type.");
            if (value is DateTime)
                this.Value = ((DateTime)value).ToString("o");
            else if (value is bool)
                this.Value = value.ToString().ToLower();
            else
                this.Value = value.ToString();
        }

        public static bool IsAllowedValue(object obj)
        {
            if (obj is string) return true;
            if (obj is DateTime) return true;
            if (obj is Geocode) return true;
            if (obj.GetType().IsPrimitiveType() == true) return true;
            return false;
        }

        public string Value { get; private set; }

        public override IEnumerable<T> GetValues<T>()
        {
            throw new Exception("StringValue does not support Values<T>().");
        }

        public override T GetValue<T>()
        {
            if (default(T) != null && this.Value == null)
                throw new Exception("Value is null.");
            return Internal.ValueConverter.Convert<T>(this.Value);
        }

        public override ValueType Type
        {
            get { return ValueType.SingleValue; }
        }

        public override bool Equals(object obj)
        {
            var other = obj as SingleValue;
            if (other == null)
                return false;
            return this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
    }

    public class MultiValue : Value
    {
        public static readonly MultiValue Empty = new MultiValue(new string[] { });

        public MultiValue(IEnumerable enumerable) 
        {
            this.Values = enumerable.Cast<object>().ToArray();
        }

        public object[] Values { get; private set; }


        public int Count
        {
            get { return this.Values.Length; }
        }

        public override ValueType Type
        {
            get { return ValueType.MultiValue; }
        }

        public override T GetValue<T>()
        {
            throw new Exception("GetValue<T> not supported for array values.");
        }

        public override IEnumerable<T> GetValues<T>()
        {
            return ValueConverter.ConvertAll<T>(this.Values);
        }

        public override bool Equals(object obj)
        {
            var other = obj as MultiValue;
            if (other == null)
                return false;
            var otherArray = other.GetValues<string>().ToArray();
            var myArray = this.GetValues<string>().ToArray();
            return otherArray.Intersect(myArray).Count() == myArray.Length;

        }

        public override int GetHashCode()
        {
            // Get sorted concatenated key
            var array = this.GetValues<string>().ToArray();
            Array.Sort(array);
            return array.AsString().GetHashCode();
        }
    }

}
