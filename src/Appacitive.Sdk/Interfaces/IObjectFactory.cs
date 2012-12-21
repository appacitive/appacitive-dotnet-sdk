using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public interface IObjectFactory
    {
        IEnumerable<T> BuildAll<T>();

        T Build<T>();

        T Build<T>(string name);
    }

    public static class ObjectFactory
    {
        public static IEnumerable<T> BuildAll<T>()
        {
            return AppacitiveContext.ObjectFactory.BuildAll<T>();
        }

        public static T Build<T>()
        {
            return AppacitiveContext.ObjectFactory.Build<T>();
        }

        public static T Build<T>(string name)
        {
            return AppacitiveContext.ObjectFactory.Build<T>(name);
        }
    }

}
