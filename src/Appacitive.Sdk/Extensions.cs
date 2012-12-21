using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    internal static class Extensions
    {
        public static void For<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            List<T> list = enumerable as List<T>;
            if (list != null)
                list.ForEach(action);
            else
            {
                foreach (var item in enumerable)
                    action(item);
            }
        }
    }
}
