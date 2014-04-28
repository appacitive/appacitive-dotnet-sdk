using Appacitive.Sdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class TypeMapping : ITypeMapping
    {
        private Dictionary<string, Type> _objectTypeMapping = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, Type> _connectionTypeMapping = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        public ITypeMapping MapObjectType<T>(string name)
            where T : APObject
        {
            _objectTypeMapping[name] = typeof(T);
            return this;
        }

        public ITypeMapping MapConnectionType<T>(string name)
            where T : APConnection
        {
            _connectionTypeMapping[name] = typeof(T);
            return this;
        }

        public Type GetMappedConnectionType(string name)
        {
            Type match = null;
            if (_connectionTypeMapping.TryGetValue(name, out match) == true)
                return match;
            else return null;

        }

        public Type GetMappedObjectType(string name)
        {
            Type match = null;
            if (_objectTypeMapping.TryGetValue(name, out match) == true)
                return match;
            else return null;
        }
    }


    public class TypeMapper
    {
        public TypeMapper()
        {
            this.Mapping = new TypeMapping();
        }

        internal ITypeMapping Mapping { get; private set; }

        public TypeMapper MapObjectType<T>(string name)
            where T : APObject
        {
            this.Mapping.MapObjectType<T>(name);
            return this;
        }

        public TypeMapper MapConnectionType<T>(string name)
            where T : APConnection
        {
            this.Mapping.MapConnectionType<T>(name);
            return this;
        }
    }
}
