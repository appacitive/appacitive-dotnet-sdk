using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Appacitive.Sdk.Realtime;

namespace Appacitive.Sdk.Internal
{
    public class EventTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(EventType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var str = string.Empty;
            if (reader.TokenType != JsonToken.Null && reader.TokenType == JsonToken.String)
                str = reader.ReadAsString();
            return NamingConvention.FromString(str);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = (EventType)value;
            writer.WriteRawValue(NamingConvention.ToString(type));

        }
    }
}
