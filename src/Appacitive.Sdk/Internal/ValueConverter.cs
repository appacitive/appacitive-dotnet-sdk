using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections;

namespace Appacitive.Sdk.Internal
{
    public static class ValueConverter
    {
        public static IEnumerable<T> ConvertAll<T>(IEnumerable enumerable )
        {
            if( enumerable == null )
                yield break;
            foreach (var item in enumerable)
                yield return Convert<T>(item);
        }   

        public static T Convert<T>( object value )
        {
            return (T)Convert(value, typeof(T));
        }

        public static object Convert(object value, Type type )
        {
            if (value == null)
            {
                if (IsNullable(type) == false)
                    throw new Exception("Given type " + type.Name + " cannot be null.");
                else
                    return null;
            }


            if (type.IsEnumeration() == true)
                return (Enum.Parse(type, value.ToString(), true));

            // Handle string to date time conversion
            if (type == typeof(DateTime))
            {
                DateTime result;
                // Return parsed date in local time zone
                if (DateTime.TryParseExact(value.ToString(), new[] { Formats.DateTime, Formats.Date }, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out result) == true)
                    return result.ToLocalTime();
                else throw new Exception("Unsupported date time format.");
            }

            // Handle Datetime to string conversion.
            if (type == typeof(string) && value is DateTime)
                return ((DateTime)value).ToString("o");

            if (type.IsPrimitiveType() == true || type == typeof(string))
                return System.Convert.ChangeType(value, type, null);
            throw new Exception("No automatic translation available for type " + type.Name + ".");
            
        }

        private static bool IsNullable(Type type)
        {
            if (type == typeof(string))
                return true;
            return false;
        }
    }
}
