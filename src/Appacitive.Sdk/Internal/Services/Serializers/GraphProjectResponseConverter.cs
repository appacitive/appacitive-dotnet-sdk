using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GraphProjectResponseConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(GraphProjectResponse) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var json = JObject.ReadFrom(reader) as JObject;
            if (json == null || json.Type == JTokenType.Null)
                return null;
            var response = new GraphProjectResponse() { Nodes = new List<GraphNode>() };
            // Parse the status
            JToken value = null;
            json.TryGetValue("status", out value);
            response.Status = serializer.Deserialize<Status>(value.CreateReader());
            if (response.Status.IsSuccessful == false)
                return response;

            // Parse the nodes
            var root = json.Properties().SingleOrDefault( p => p.Name != "status");
            if (root == null || root.Value.Type != JTokenType.Object)
                return response;
            var rootObject = root.Value as JObject;
            if (rootObject == null)
                return response;
            var valuesProperty = rootObject.Property("values");
            if (valuesProperty == null || valuesProperty.Value.Type != JTokenType.Array)
                return response;
            var nodeJsons = valuesProperty.Values().Select(x => x as JObject);
            foreach (var nodeJson in nodeJsons)
            {
                var node = serializer.Deserialize<GraphNode>(nodeJson.CreateReader());
                response.Nodes.Add(node);
            }


            return response;    
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
