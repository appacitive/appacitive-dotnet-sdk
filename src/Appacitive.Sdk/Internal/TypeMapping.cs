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
            ValidateObjectTypeMapping<T>();
            _objectTypeMapping[name] = typeof(T);
            return this;
        }

        public ITypeMapping MapConnectionType<T>(string name)
            where T : APConnection
        {
            ValidateConnectionTypeMapping<T>();
            _connectionTypeMapping[name] = typeof(T);
            return this;
        }

        private void ValidateObjectTypeMapping<T>()
        {
            // Check that type has required constructor.
            var constructors = typeof(T).GetConstructors();
            var match = constructors.Where(c =>
                {
                    if (c.IsPublic == false) return false;
                    var parameters = c.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(APObject))
                        return true;
                    else return false;
                }).SingleOrDefault();
            if (match == null)
                throw new AppacitiveRuntimeException("Subclassed type must have a public constructor with a parameter of type APObject.");
        }

        private void ValidateConnectionTypeMapping<T>()
        {
            // Check that type has required constructor.
            var constructors = typeof(T).GetConstructors();
            var match = constructors.Where(c =>
            {
                if (c.IsPublic == false) return false;
                var parameters = c.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType == typeof(APConnection))
                    return true;
                else return false;
            }).SingleOrDefault();
            if (match == null)
                throw new AppacitiveRuntimeException("Subclassed type must have a public constructor with a parameter of type APConnection.");
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
}
