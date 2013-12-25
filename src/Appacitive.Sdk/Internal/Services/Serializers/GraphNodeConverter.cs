using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GraphNodeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(GraphNode) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = JObject.ReadFrom(reader);
            return ParseGraphNode(null, null, null, value as JObject, serializer);
        }

        private GraphNode ParseGraphNode(GraphNode parent, string name, string parentLabel, JObject json, JsonSerializer serializer)
        {
            GraphNode current = new GraphNode();
            // Parse the object
            var objectJson = ExtractObjectJson(json);
            current.Object = serializer.Deserialize<APObject>(objectJson.CreateReader());
            // Parse edge
            if (parent != null)
            {
                var edgeJson = ExtractEdgeJson(json);
                if (edgeJson != null)
                    current.Connection = ParseConnection(parentLabel, parent.Object, current.Object, edgeJson);
            }
            // Parse children
            ParseChildNodes(json, current, serializer);
            // Set parent context
            if (parent != null)
            {
                parent.AddChildNode(name, current);
                current.Parent = parent;
            }
            return current;
        }

        private void ParseChildNodes(JObject json, GraphNode current, JsonSerializer serializer)
        {
            JToken value = null;
            if (json.TryGetValue("__children", out value) == false || value.Type != JTokenType.Object)
                return;
            foreach (var property in ((JObject)value).Properties())
            {
                if (property.Value.Type != JTokenType.Object) continue;
                ParseChildNodes(property.Name, property.Value as JObject, current, serializer);
            }
        }

        private void ParseChildNodes(string name, JObject json, GraphNode current, JsonSerializer serializer)
        {
            var parentLabel = GetValue(json, "parent", JTokenType.String, true).ToString();
            var values = json.Property("values");
            if (values.Value.Type == JTokenType.Array)
            {
                var nodeJsons = values.Values().Select(x => x as JObject);
                foreach (var nodeJson in nodeJsons)
                    ParseGraphNode(current, name, parentLabel, nodeJson, serializer);
            }
        }

        private APConnection ParseConnection(string parentLabel, APObject parentObj, APObject currentObj, JObject json)
        {
            string label = string.Empty;
            if( json.Property("__label") != null ) 
                label = GetValue(json, "__label", JTokenType.String, true).ToString();
            else 
                label = GetValue(json, "label", JTokenType.String, true).ToString();
            var relation = GetValue(json, "__relationtype", JTokenType.String, true).ToString();
            var id = GetValue(json, "__id", JTokenType.String, true).ToString();
            var conn = new APConnection(relation, id);
            conn.Endpoints = new EndpointPair(
                new Endpoint(parentLabel, parentObj) { ObjectId = parentObj.Id },
                new Endpoint(label, currentObj) { ObjectId = currentObj.Id });
            // Parse system properties
            JToken value = null;
            // Id
            if (json.TryGetValue("__id", out value) == true && value.Type != JTokenType.Null)
            {
                conn.Id = value.ToString();
                json.Remove("__id");
            }
            // Revision
            if (json.TryGetValue("__revision", out value) == true && value.Type != JTokenType.Null)
            {
                conn.Revision = int.Parse(value.ToString());
                json.Remove("__revision");
            }
            // Created by
            if (json.TryGetValue("__createdby", out value) == true && value.Type != JTokenType.Null)
            {
                conn.CreatedBy = value.ToString();
                json.Remove("__createdby");
            }
            // Create date
            if (json.TryGetValue("__utcdatecreated", out value) == true && value.Type != JTokenType.Null)
            {
                conn.CreatedAt = (DateTime)value;
                json.Remove("__utcdatecreated");
            }
            // Last updated by
            if (json.TryGetValue("__lastmodifiedby", out value) == true && value.Type != JTokenType.Null)
            {
                conn.LastUpdatedBy = value.ToString();
                json.Remove("__lastmodifiedby");
            }
            // Last update date
            if (json.TryGetValue("__utclastupdateddate", out value) == true && value.Type != JTokenType.Null)
            {
                conn.LastUpdatedAt = (DateTime)value;
                json.Remove("__utclastupdateddate");
            }

            // Parse connection tags
            if (json.TryGetValue("__tags", out value) == true && value.Type == JTokenType.Array)
            {
                ((JArray)value).Values<string>().For(t => conn.AddTag(t, true));
            }
            json.Remove("__tags");

            // Parse connection attributes
            if (json.TryGetValue("__attributes", out value) == true && value.Type == JTokenType.Object)
            {
                foreach (var property in ((JObject)value).Properties())
                {
                    if( property.Type != JTokenType.Null )
                        conn.SetAttribute(property.Name, property.Value.ToString(), true);
                }
            }
            json.Remove("__attributes");

            // properties
            foreach (var property in json.Properties())
            {
                // Ignore objects
                if (property.Value.Type == JTokenType.Object) continue;
                // Check for arrays
                else if (property.Value.Type == JTokenType.Array)
                {
                    conn.SetList<string>(property.Name, property.Value.Values<string>(), true);
                }
                // Set value of the property
                else if (property.Value.Type == JTokenType.Date)
                    conn.SetField(property.Name, ((DateTime)property.Value).ToString("o"), true);
                else
                    conn.SetField(property.Name, property.Value.Type == JTokenType.Null ? null : property.Value.ToString(), true);
            }

            return conn;
        }

        private JToken GetValue(JObject json, string propertyName, JTokenType expectedType, bool removeProperty = false)
        {
            JToken value;
            if( json.TryGetValue(propertyName, out value) == false || value.Type != expectedType )
            {
                var exception = new Exception("Json is not valid graph node json.");
                exception.Data["json"] = json.ToString();
                throw exception;
            }
            if (removeProperty == true)
                json.Remove(propertyName);
            return value;
        }

        private JObject ExtractEdgeJson(JObject json)
        {
            JToken value = null;
            if (json.TryGetValue("__edge", out value) == false)
                return null;
            if (value.Type == JTokenType.Null)
                return null;
            return value.DeepClone() as JObject;
        }

        private JObject ExtractObjectJson(JObject json)
        {
            var clone = json.DeepClone() as JObject;
            var properties = clone.Properties().ToArray();
            for (int i = 0; i < properties.Length; i++)
            {
                if( IsObjectProperty(properties[i].Name) == false )
                    clone.Remove(properties[i].Name);
            }
            return clone;
        }

        private bool IsObjectProperty(string propertyName)
        {
            if (propertyName.Equals("__edge", StringComparison.OrdinalIgnoreCase) == true)
                return false;
            if (propertyName.Equals("__children", StringComparison.OrdinalIgnoreCase) == true)
                return false;
            return true;
        }

        

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
