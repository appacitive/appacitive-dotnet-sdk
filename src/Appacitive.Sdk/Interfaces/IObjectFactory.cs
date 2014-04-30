using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    /// <summary>
    /// Dependency container interface.
    /// </summary>
    public interface IDependencyContainer
    {
        /// <summary>
        /// Build an instance for all types registered with the given type T.
        /// </summary>
        /// <typeparam name="T">The interface type</typeparam>
        /// <returns>Collection of object instances of type T.</returns>
        IEnumerable<T> BuildAll<T>() where T : class;

        /// <summary>
        /// Builds and instance corresponding to the concrete type registered with given type T.
        /// </summary>
        /// <typeparam name="T">The interface type.</typeparam>
        T Build<T>() where T : class;

        /// <summary>
        /// Builds and instance corresponding to the concrete type registered with given type T and the given name.
        /// </summary>
        /// <typeparam name="T">The interface type.</typeparam>
        T Build<T>(string name) where T : class;

        /// <summary>
        /// Registers a interface vs concrete type mapping inside the dependency container.
        /// </summary>
        /// <typeparam name="TInterface">Interface type</typeparam>
        /// <typeparam name="TImpl">Concrete type</typeparam>
        IDependencyContainer Register<TInterface, TImpl>() where TImpl : TInterface;

        /// <summary>
        /// Registers a interface vs concrete type mapping inside the dependency container with a specific name.
        /// </summary>
        /// <typeparam name="TInterface">Interface type</typeparam>
        /// <typeparam name="TImpl">Concrete type</typeparam>
        /// <param name="name">Mapping name.</param>
        IDependencyContainer Register<TInterface, TImpl>(string name) where TImpl : TInterface;

        /// <summary>
        /// Registers a interface with a factory to create its corresponding concrete instance.
        /// </summary>
        /// <typeparam name="TInterface">Interface type</typeparam>
        /// <param name="factory">Concrete type factory</param>
        IDependencyContainer Register<TInterface, TImpl>(Func<TImpl> factory) where TImpl : TInterface;

        /// <summary>
        /// Registers a interface with a factory to create its corresponding concrete instance with a specific name.
        /// </summary>
        /// <typeparam name="TInterface">Interface type</typeparam>
        /// <param name="factory">Concrete type factory</param>
        /// <param name="name">Mapping name.</param>
        IDependencyContainer Register<TInterface, TImpl>(string name, Func<TImpl> factory) where TImpl : TInterface;
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
