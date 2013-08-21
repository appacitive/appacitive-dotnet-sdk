using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    internal static class Guard
    {
        private static readonly Type[] AllowedTypes = new Type[]
        {
            //typeof(Value),
            typeof(int), typeof(long), typeof(decimal), typeof(float), typeof(double), typeof(string), typeof(DateTime), typeof(bool),
            typeof(int[]), typeof(long[]), typeof(decimal[]), typeof(float[]), typeof(double[]), typeof(string[]), typeof(DateTime[]), typeof(bool[]),
            typeof(List<int>), typeof(List<long>), typeof(List<decimal>), typeof(List<float>), typeof(List<double>), typeof(List<string>), typeof(List<DateTime>), typeof(List<bool>),
            typeof(Geocode)
        };

        private static readonly Type[] AllowedPrimitiveTypes = new Type[]
        {
            //typeof(Value),
            typeof(int), typeof(long), typeof(decimal), typeof(float), typeof(double), typeof(string), typeof(DateTime), typeof(bool),
            typeof(Geocode)
        };

        public static void ValidateAllowedPrimitiveTypes(Type type)
        {
            // Should be a value type or a list of value types
            if (AllowedPrimitiveTypes.Contains(type) == false)
                throw ErrorSpace.PropertyTypeIsNotSupported(type);

        }

        public static void ValidateAllowedTypes(Type type )
        {
            // Should be a value type or a list of value types
            if (AllowedTypes.Contains(type) == false)
                throw ErrorSpace.PropertyTypeIsNotSupported(type);

        }
    }
}
