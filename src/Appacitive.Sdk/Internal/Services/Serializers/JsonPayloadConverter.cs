using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Appacitive.Sdk.Internal
{
    public class JsonObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Is<IJsonObject>();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            var json = JObject.ReadFrom(reader) as JObject;
            return new JsonObject(json);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var json = value as IJsonObject;
            if (json == null)
            {
                writer.WriteNull();
                return;
            }
            else 
            {
                var obj = JObject.Parse(json.AsString());
                obj.WriteTo(writer);
            }
        }
    }
}
