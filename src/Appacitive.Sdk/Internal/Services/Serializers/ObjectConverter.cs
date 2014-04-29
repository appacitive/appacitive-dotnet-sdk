using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Appacitive.Sdk.Internal;

namespace Appacitive.Sdk.Services
{   
    public class ObjectConverter : EntityConverter
    {
        public override bool CanConvert(Type objectType)
        {
            // Type should not be a User or Device since these have their specific serializers.
            // This serializer should be used for any other type that inherits from object.
            if (objectType.Is<APUser>() == true || objectType.Is<APDevice>() == true)
                return false;
            return objectType.Is<APObject>();
        }

        protected override Entity CreateEntity(JObject json)
        {
            JToken value;
            if (json.TryGetValue("__type", out value) == false || value.Type == JTokenType.Null)
                throw new Exception("Schema type missing.");
            var type = value.ToString();
            var mappedType = InternalApp.Types.Mapping.GetMappedObjectType(type);
            if (mappedType == null)
                return new APObject(type);
            else 
                return Activator.CreateInstance(mappedType) as Entity;
        }

        protected override Entity ReadJson(Entity entity, Type objectType, JObject json, JsonSerializer serializer)
        {
            if (json == null || json.Type == JTokenType.Null)
                return null;
            // JToken value;
            var obj = base.ReadJson(entity, objectType, json, serializer) as APObject;
            return obj;
        }

        protected override void WriteJson(Entity entity, JsonWriter writer, JsonSerializer serializer)
        {
            if (entity == null)
                return;
            var obj = entity as APObject;
            if (obj != null)
            {
                writer
                    .WriteProperty("__type", obj.Type);
                    //.WriteProperty("__schemaid", obj.SchemaId);
            }
        }

        private static readonly Dictionary<string, bool> _internal = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
        {
            {"__schematype", true},
            {"__schemaid", true}
        };

        protected override bool IsSytemProperty(string property)
        {
            if (base.IsSytemProperty(property) == true)
                return true;
            else
                return _internal.ContainsKey(property);
            
        }
    }
}
