using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static class ErrorSpace
    {
        public static Exception PropertyTypeIsNotSupported(Type type)
        {
            return new AppacitiveRuntimeException(string.Format("Values of type {0} cannot be assigned to object properties.", type.Name));
        }
    }
}
