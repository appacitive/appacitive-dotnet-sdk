using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Appacitive.Sdk.Services
{
    public class UpdateDeviceRequestConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(UpdateDeviceRequest) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var request = value as UpdateDeviceRequest;
            if (request == null)
            {
                writer.WriteNull();
                return;
            }
            
            writer
                .StartObject()
                // Write properties
                .WithWriter(w => request.PropertyUpdates.For(p => w.WriteProperty(p.Key, p.Value)))
                // Write atttributes
                .WithWriter(w =>
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
                .WithWriter(w =>
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
