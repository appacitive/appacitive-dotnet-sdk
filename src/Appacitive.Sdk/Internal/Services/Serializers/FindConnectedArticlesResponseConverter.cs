using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class FindConnectedArticlesResponseConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(FindConnectedArticlesResponse) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken value = null;
            var response = new FindConnectedArticlesResponse();
            // status
            var json = JObject.ReadFrom(reader) as JObject;
            json.TryGetValue("status", out value);
            response.Status = serializer.Deserialize<Status>(value.CreateReader());
            if (response.Status.IsSuccessful == false)
                return response;
            json.Remove("status");

            // paging info
            // Extract paging info
            json.TryGetValue("paginginfo", out value);
            response.PagingInfo = serializer.Deserialize<PagingInfo>(value.CreateReader());
            json.Remove("paginginfo");

            // extract parent label
            json.TryGetValue("parent", out value);
            var parentLabel = value.ToString();

            // Extract graph node.
            json.TryGetValue("nodes", out value);
            if (value.Type != JTokenType.Null)
            {
                var nodes = value.Values<JObject>();
                ParseNodes(response, parentLabel, nodes, serializer);
            }
            else
                response.Nodes = new List<GraphNode>();
            return response;
        }

        private void ParseNodes(FindConnectedArticlesResponse response, string parentLabel, IEnumerable<JObject> nodes, JsonSerializer serializer)
        {
            response.Nodes = nodes.Select(x => ParseNode(parentLabel, x, serializer)).ToList();
        }

        private GraphNode ParseNode( string parentLabel, JObject json, JsonSerializer serializer)
        {
            GraphNode current = new GraphNode();
            // Parse the article
            var articleJson = ExtractArticleJson(json);
            current.Article = serializer.Deserialize<Article>(articleJson.CreateReader());

            var edgeJson = ExtractEdgeJson(json);
            if (edgeJson != null)
                current.Connection = ParseConnection(parentLabel, null, current.Article, edgeJson);

            return current;
        }

        private Connection ParseConnection(string parentLabel, Article parentArticle, Article currentArticle, JObject json)
        {
            string label = string.Empty;
            if( json.Property("__label") != null ) 
                label = GetValue(json, "__label", JTokenType.String, true).ToString();
            else 
                label = GetValue(json, "label", JTokenType.String, true).ToString();
            var relation = GetValue(json, "__relationtype", JTokenType.String, true).ToString();
            var id = GetValue(json, "__id", JTokenType.String, true).ToString();
            var conn = new Connection(relation, id);
            conn.Endpoints = new EndpointPair(
                new Endpoint(parentLabel, parentArticle),
                new Endpoint(label, currentArticle));
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
                conn.UtcCreateDate = (DateTime)value;
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
                conn.UtcLastUpdated = (DateTime)value;
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

        private JObject ExtractArticleJson(JObject json)
        {
            var clone = json.DeepClone() as JObject;
            var properties = clone.Properties().ToArray();
            for (int i = 0; i < properties.Length; i++)
            {
                if( IsArticleProperty(properties[i].Name) == false )
                    clone.Remove(properties[i].Name);
            }
            return clone;
        }

        private bool IsArticleProperty(string propertyName)
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
