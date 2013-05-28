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
    public class RealTimeMessageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            #if !WINDOWS_PHONE7
            return typeof(RealTimeMessage).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
            #else
            return typeof(RealTimeMessage).IsAssignableFrom(objectType);
            #endif

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            // Read the message type and serialize accordingly
            JToken value;
            string typeCode = null;
            var json = JObject.ReadFrom(reader) as JObject;
            if (json.TryGetValue("mt", out value) == true && value.Type == JTokenType.String)
                typeCode = value.ToString();
            var type = RealTimeMessage.GetType(typeCode);
            if (type != null)
                return new JsonSerializer().Deserialize(json.CreateReader(), RealTimeMessage.GetType(typeCode));
            else return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            new JsonSerializer().Serialize(writer, value);
        }
    }
}
