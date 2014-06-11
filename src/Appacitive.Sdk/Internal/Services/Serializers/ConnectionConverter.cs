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
using System.IO;

namespace Appacitive.Sdk.Services
{
    public class ConnectionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Is<APConnection>();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Read the type of the object and create  a new instance.
            // Invoke a helper to populate the common fields for the entity
            // Parse the rest
            var json = JObject.ReadFrom(reader) as JObject;
            if (json == null || json.Type == JTokenType.Null)
                return null;
            var conn = BuildNewInstance(json, objectType);
            EntityParser.ReadJson(conn, json, serializer);
            // The only field only available in connection ie, endpoints
            ParseEndpoints(conn, json, serializer);
            return conn;
        }

        private void ParseEndpoints(APConnection conn, JObject json, JsonSerializer serializer)
        {
            // Parse the endpoints
            JToken value;
            Endpoint ep1 = null, ep2 = null;
            if (json.TryGetValue("__endpointa", out value) == true && value.Type == JTokenType.Object)
                ep1 = ParseEndpoint(value as JObject, serializer);
            else throw new Exception(string.Format("Endpoint A for connection with id {0} is invalid.", conn.Id));
            if (json.TryGetValue("__endpointb", out value) == true && value.Type == JTokenType.Object)
                ep2 = ParseEndpoint(value as JObject, serializer);
            else throw new Exception(string.Format("Endpoint B for connection with id {0} is invalid.", conn.Id));
            conn.Endpoints = new EndpointPair(ep1, ep2);
        }

        private Endpoint ParseEndpoint(JObject json, JsonSerializer serializer)
        {
            if (json == null)
                return null;
            string label = null, objectId = null;
            string type = null;
            JToken value;
            // Parse the label
            if (json.TryGetValue("label", out value) == true && value.Type != JTokenType.Null)
                label = value.ToString();
            // Parse the object type
            if (json.TryGetValue("type", out value) == true && value.Type != JTokenType.Null)
                type = value.ToString();
            // Parse the object id
            if (json.TryGetValue("objectid", out value) == true && value.Type != JTokenType.Null)
                objectId = value.ToString();
            // Parse the object
            APObject obj = null;
            if (json.TryGetValue("object", out value) == true && value.Type != JTokenType.Null && value.Type == JTokenType.Object)
            {
                using (var reader = value.CreateReader())
                {
                    obj = serializer.Deserialize<APObject>(value.CreateReader());
                }
            }
            if (string.IsNullOrWhiteSpace(label) == true)
                throw new Exception("Endpoint label is missing.");
            if (string.IsNullOrWhiteSpace(objectId) == true)
                throw new Exception("Endpoint object id is missing.");

            return new Endpoint(label, objectId) { Content = obj, Type = type };
        }

        private APConnection BuildNewInstance(JObject json, Type objectType)
        {
            var type = GetType(json);
            var mappedType = InternalApp.Types.Mapping.GetMappedObjectType(type);
            if (mappedType == null)
                return new APConnection(type);
            else
                return Activator.CreateInstance(mappedType) as APConnection;
        }

        private string GetType(JObject json)
        {
            JToken value;
            if (json.TryGetValue("__relationtype", out value) == false || value.Type == JTokenType.Null)
                throw new Exception("Relation type missing.");
            return value.ToString();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Manage the open and closing of the object.
            // Write APConnection specific content (e.g., endpoints)
            // Call appropriate helpers to write common stuff.
            APConnection conn = value as APConnection;
            if (conn == null)
            {
                writer.WriteNull();
                return;
            }
            writer.StartObject();
            EntityParser.WriteJson(writer, conn, serializer);
            WriteEndpoints(writer, conn, serializer);
            writer.WriteEndObject();
        }

        private void WriteEndpoints(JsonWriter writer, APConnection conn, JsonSerializer serializer)
        {
            // Write endpoint A
            if (conn.Endpoints.EndpointA.CreateEndpoint == false)
            {
                writer
                    .WriteProperty("__endpointa")
                    .StartObject()
                    .WriteProperty("label", conn.Endpoints.EndpointA.Label)
                    .WriteProperty("objectid", conn.Endpoints.EndpointA.ObjectId)
                    .EndObject();
            }
            else
            {
                writer
                    .WriteProperty("__endpointa")
                    .StartObject()
                    .WriteProperty("label", conn.Endpoints.EndpointA.Label)
                    .WriteProperty("object")
                    .WithWriter(w => WriteObject(w, conn.Endpoints.EndpointA.Content))
                    .EndObject();
            }

            // Write endpoint B
            if (conn.Endpoints.EndpointB.CreateEndpoint == false)
            {
                writer
                    .WriteProperty("__endpointb")
                    .StartObject()
                    .WriteProperty("label", conn.Endpoints.EndpointB.Label)
                    .WriteProperty("objectid", conn.Endpoints.EndpointB.ObjectId)
                    .EndObject();
            }
            else
            {
                writer
                    .WriteProperty("__endpointb")
                    .StartObject()
                    .WriteProperty("label", conn.Endpoints.EndpointB.Label)
                    .WriteProperty("object")
                    .WithWriter(w => WriteObject(w, conn.Endpoints.EndpointB.Content))
                    .EndObject();
            }
        }

        private void WriteObject(JsonWriter writer, APObject obj)
        {
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            var bytes = serializer.Serialize(obj);
            using (var reader = new StreamReader(new MemoryStream(bytes), Encoding.UTF8))
            {
                writer.WriteRawValue(reader.ReadToEnd());
            }
        }
    }
}


