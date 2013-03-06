using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public interface IDependencyContainer
    {
        IEnumerable<T> BuildAll<T>() where T : class;

        T Build<T>() where T : class;

        T Build<T>(string name) where T : class;

        IDependencyContainer Register<TInterface, TImpl>() where TImpl : TInterface;

        IDependencyContainer Register<TInterface, TImpl>(string name) where TImpl : TInterface;

        IDependencyContainer Register<TInterface, TImpl>(Func<object> factory) where TImpl : TInterface;

        IDependencyContainer Register<TInterface, TImpl>(string name, Func<object> factory) where TImpl : TInterface;
    }

    public static class ObjectFactory
    {
        public static IEnumerable<T> BuildAll<T>()
            where T : class
        {
            return AppacitiveContext.ObjectFactory.BuildAll<T>();
        }

        public static T Build<T>()
            where T : class
        {
            return AppacitiveContext.ObjectFactory.Build<T>();
        }

        public static T Build<T>(string name)
            where T : class
        {
            return AppacitiveContext.ObjectFactory.Build<T>(name);
        }
    }

}
