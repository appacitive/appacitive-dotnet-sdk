using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
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

    public static class IDependencyContainerExtensions
    {
        public static IDependencyContainer RegisterInstance<TInterface, TImpl>(this IDependencyContainer container, TImpl instance)
            where TImpl : TInterface
        {
            return container.Register<TInterface, TImpl>(() => instance);
        }

        public static IDependencyContainer RegisterInstance<TInterface, TImpl>(this IDependencyContainer container, string name, TImpl instance)
            where TImpl : TInterface
        {
            return container.Register<TInterface, TImpl>(name, () => instance);
        }
            
            
    }
}
