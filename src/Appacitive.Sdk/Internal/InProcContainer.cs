using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class InProcContainer : IDependencyContainer
    {
        private InProcContainer()
        {   
        }

        public static InProcContainer Instance = new InProcContainer();
        
        private Dictionary<TypeKey, Func<Object>> _factories = new Dictionary<TypeKey, Func<object>>();

        public IEnumerable<T> BuildAll<T>()
            where T : class
        {
            foreach (var key in _factories.Keys)
            {
                if (typeof(T) == key.Type)
                {
                    Func<object> factory = null;
                    if (_factories.TryGetValue(key, out factory) == true)
                        yield return factory() as T;
                }
            }
        }

        public T Build<T>()
            where T : class
        {
            return Build<T>(null);
        }

        public T Build<T>(string name)
            where T : class
        {
            Func<object> createNew = null;
            if (_factories.TryGetValue(new TypeKey(typeof(T), name), out createNew) == false)
                throw new Exception("Type registration for " + typeof(T).Name + " is missing.");
            else
                return createNew() as T;
        }

        private Func<Object> CreateDefault(Type type)
        {
            return () => Activator.CreateInstance(type);
        }

        public InProcContainer Register<TInterface, TImpl>()
            where TImpl : TInterface
        {
            string name = null;
            return Register<TInterface, TImpl>(name);
        }

        public InProcContainer Register<TInterface, TImpl>(string name)
            where TImpl : TInterface
        {
            return Register<TInterface, TImpl>(null, CreateDefault(typeof(TImpl)));
        }

        public InProcContainer Register<TInterface, TImpl>(Func<object> factory)
            where TImpl : TInterface
        {
            return Register<TInterface, TImpl>(null, factory);
        }

        public InProcContainer Register<TInterface, TImpl>(string name, Func<object> factory)
            where TImpl : TInterface
        {
            _factories[new TypeKey(typeof(TInterface), name)] = factory;
            return this;
        }

        IEnumerable<T> IDependencyContainer.BuildAll<T>()
        {
            return this.BuildAll<T>();
        }

        T IDependencyContainer.Build<T>()
        {
            return this.Build<T>();
        }

        T IDependencyContainer.Build<T>(string name)
        {
            return this.Build<T>(name);
        }

        IDependencyContainer IDependencyContainer.Register<TInterface, TImpl>()
        {
            return this.Register<TInterface, TImpl>();
        }

        IDependencyContainer IDependencyContainer.Register<TInterface, TImpl>(string name)
        {
            return this.Register<TInterface, TImpl>(name);
        }

        IDependencyContainer IDependencyContainer.Register<TInterface, TImpl>(Func<TImpl> factory)
        {
            return this.Register<TInterface, TImpl>(() => factory());
        }

        IDependencyContainer IDependencyContainer.Register<TInterface, TImpl>(string name, Func<TImpl> factory)
        {
            return this.Register<TInterface, TImpl>(name, () => factory());
        }
    }

    internal class TypeKey
    {
        public TypeKey(Type type, string name)
        {
            this.Type = type;
            if( this.Name != null )
                this.Name = name.ToLower();
        }

        public string Name { get; private set; }

        public Type Type { get; private set; }

        public override bool Equals(object obj)
        {
            var other = obj as TypeKey;
            if (other == null)
                return false;
            return this.Name == other.Name && this.Type == other.Type;
        }

        public override int GetHashCode()
        {
            if (this.Name == null)
                return this.Type.GetHashCode();
            else
                return this.Type.GetHashCode() ^ this.Name.GetHashCode();
        }
    }


}
