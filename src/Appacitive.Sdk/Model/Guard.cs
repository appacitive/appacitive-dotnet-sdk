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
            typeof(int), typeof(long), typeof(decimal), typeof(float), typeof(double), typeof(string), typeof(DateTime), typeof(bool),
            typeof(Value)
        };

        public static void ValidateAllowedTypes(Type type )
        {
            if (AllowedTypes.Contains(type) == false)
                throw ErrorSpace.PropertyTypeIsNotSupported(type);
        }
    }
}
