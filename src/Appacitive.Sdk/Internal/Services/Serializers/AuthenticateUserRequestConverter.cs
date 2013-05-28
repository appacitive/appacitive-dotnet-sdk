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
    public class AuthenticateUserRequestConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(AuthenticateUserRequest) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var request = value as AuthenticateUserRequest;
            if (request == null)
            {
                writer.WriteNull();
                return;
            }

            writer
                .StartObject()
                .WithWriter(w =>
                {
                    if (string.IsNullOrWhiteSpace(request.Type) == false)
                        w.WriteProperty("type", request.Type);
                    if (request.MaxAttempts > 0)
                        w.WriteProperty("attempts").WriteValue(request.MaxAttempts);
                    if (request.TimeoutInSeconds > 0)
                        w.WriteProperty("expiry").WriteValue(request.TimeoutInSeconds);
                    if (request.CreateUserIfNotExists)
                        w.WriteProperty("createnew").WriteValue(request.CreateUserIfNotExists);
                    foreach (var key in request.Attributes.Keys)
                        w.WriteProperty(key, request[key]);
                })
                .EndObject();
        }
    }

    
}
