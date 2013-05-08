using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class DeviceConverter : EntityConverter
    {
        protected override Entity CreateEntity(JObject json)
        {
            JToken value;
            string type = string.Empty;
            if (json.TryGetValue("devicetype", out value) == true && value.Type != JTokenType.Null)
                type = value.ToString();
            else throw new Exception("DeviceType not present in response.");
            return new Device( SupportedDevices.ResolveDeviceType(type) );
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Device) == objectType;
        }

        protected override Entity ReadJson(Entity entity, Type objectType, JObject json, JsonSerializer serializer)
        {
            if (json == null || json.Type == JTokenType.Null)
                return null;
            JToken value;
            var device = base.ReadJson(entity, objectType, json, serializer) as Device;
            if (device != null)
            {
                // Schema Id
                if (json.TryGetValue("__schemaid", out value) == true && value.Type != JTokenType.Null)
                    device.SchemaId = value.ToString();
                // Revision
                if (json.TryGetValue("__revision", out value) == true && value.Type != JTokenType.Null)
                    device.Revision = int.Parse(value.ToString());
            }
            return device;
        }

        protected override void WriteJson(Entity entity, JsonWriter writer, JsonSerializer serializer)
        {
            if (entity == null)
                return;
            var device = entity as Device;
            if (device != null)
            {
                writer
                    .WriteProperty("__schemaid", device.SchemaId);
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
