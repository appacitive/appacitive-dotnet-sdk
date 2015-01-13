using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace Appacitive.Sdk.Services
{
    public class UpdateConnectionRequestConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(UpdateConnectionRequest) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private void WriteProperty(JsonWriter writer, string key, object property)
        {
            writer.WritePropertyName(key);
            if (property != null && property is string == false && property is IEnumerable)
            {
                var enumerable = property as IEnumerable;
                writer.WriteStartArray();
                foreach (var item in enumerable)
                    writer.WriteValue(item.ToString());
                writer.WriteEndArray();
            }
            else
            {
                if (property == null)
                    writer.WriteNull();
                else
                    writer.WriteValue(property.ToString());
            }

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var request = value as UpdateConnectionRequest;
            if (request == null)
            {
                writer.WriteNull();
                return;
            }
            
            writer
                .StartObject()
                // Write id and type
                .WriteProperty("__id", request.Id)
                .WriteProperty("__relationtype", request.Type)
                // Write properties
                .WithWriter( w => request.PropertyUpdates.For( p => WriteProperty(w, p.Key, p.Value)))
                // Write atttributes
                .WithWriter( w => 
                    {
                        if (request.AttributeUpdates.Count > 0)
                        {
                            w.WriteProperty("__attributes")
                             .StartObject()
                             .WithWriter(w2 => request.AttributeUpdates.For(a => w2.WriteProperty(a.Key, a.Value)))
                             .EndObject();
                        }
                    })
                // Write add tags
                .WithWriter( w => 
                    {
                        if (request.AddedTags.Count > 0)
                            w.WriteArray("__addtags", request.AddedTags);
                    })
                // Write remove tags
                .WithWriter(w =>
                {
                    if (request.RemovedTags.Count > 0)
                        w.WriteArray("__removetags", request.RemovedTags);
                })
                .EndObject();
        }
    }

}
