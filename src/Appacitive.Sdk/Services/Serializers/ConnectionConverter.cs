using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Appacitive.Sdk.Interfaces;

namespace Appacitive.Sdk.Services
{
    public class ConnectionConverter : EntityConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Connection);
        }

        protected override void WriteJson(Entity entity, JsonWriter writer, JsonSerializer serializer)
        {
            var conn = entity as Connection;
            if (conn == null)
                return;

            
            // Write endpoint A
            if (conn.Endpoints.EndpointA.CreateEndpoint == false)
            {
                writer
                    .WriteProperty("__endpointa")
                    .StartObject()
                    .WriteProperty("label", conn.Endpoints.EndpointA.Label)
                    .WriteProperty("articleid", conn.Endpoints.EndpointA.ArticleId)
                    .EndObject();
            }
            else
            {
                writer
                    .WriteProperty("__endpointa")
                    .StartObject()
                    .WriteProperty("label", conn.Endpoints.EndpointA.Label)
                    .WriteProperty("article")
                    .WithWriter( w => WriteArticle(w, conn.Endpoints.EndpointA.Content) )
                    .EndObject();
            }

            // Write endpoint B
            if (conn.Endpoints.EndpointB.CreateEndpoint == false)
            {
                writer
                    .WriteProperty("__endpointb")
                    .StartObject()
                    .WriteProperty("label", conn.Endpoints.EndpointB.Label)
                    .WriteProperty("articleid", conn.Endpoints.EndpointB.ArticleId)
                    .EndObject();
            }
            else
            {
                writer
                    .WriteProperty("__endpointb")
                    .StartObject()
                    .WriteProperty("label", conn.Endpoints.EndpointB.Label)
                    .WriteProperty("article")
                    .WithWriter(w => WriteArticle(w, conn.Endpoints.EndpointB.Content))
                    .EndObject();
            }
        }

        protected override Entity CreateEntity(JObject json)
        {
            JToken value;
            if (json.TryGetValue("__relationtype", out value) == false || value.Type == JTokenType.Null)
                throw new Exception("Relation type missing.");
            var type = value.ToString();
            return new Connection(type);
        }

        protected override Entity ReadJson(Entity entity, Type objectType, Newtonsoft.Json.Linq.JObject json, JsonSerializer serializer)
        {
            var conn = base.ReadJson(entity, objectType, json, serializer) as Connection;
            if (conn == null)
                return null;
            // Parse the relation information
            // Relation id
            JToken value;
            if (json.TryGetValue("__relationid", out value) == true && value.Type != JTokenType.Null)
                conn.RelationId = value.ToString();

            // Parse the endpoints
            Endpoint ep1 = null, ep2 = null;
            if (json.TryGetValue("__endpointa", out value) == true && value.Type == JTokenType.Object)
                ep1 = ParseEndpoint(value as JObject, serializer);
            else throw new Exception(string.Format("Endpoint A for connection with id {0} is invalid.", conn.Id));
            if (json.TryGetValue("__endpointb", out value) == true && value.Type == JTokenType.Object)
                ep2 = ParseEndpoint(value as JObject, serializer);
            else throw new Exception(string.Format("Endpoint B for connection with id {0} is invalid.", conn.Id));
            conn.Endpoints = new EndpointPair(ep1, ep2);
            return conn;
        }

        private Endpoint ParseEndpoint(JObject json, JsonSerializer serializer)
        {
            if (json == null)
                return null;
            string label = null, articleId = null;
            string type = null;
            JToken value;
            // Parse the label
            if (json.TryGetValue("label", out value) == true && value.Type != JTokenType.Null)
                label = value.ToString();
            // Parse the article type
            if (json.TryGetValue("type", out value) == true && value.Type != JTokenType.Null)
                type = value.ToString();
            // Parse the article id
            if (json.TryGetValue("articleid", out value) == true && value.Type != JTokenType.Null)
                articleId = value.ToString();
            // Parse the article
            Article article = null;
            if (json.TryGetValue("article", out value) == true && value.Type != JTokenType.Null && value.Type == JTokenType.Object)
            {
                using (var reader = value.CreateReader())
                {
                    article = serializer.Deserialize<Article>(value.CreateReader());
                }
            }
            if( string.IsNullOrWhiteSpace(label) == true )
                throw new Exception("Endpoint lable is missing.");
            if (string.IsNullOrWhiteSpace(articleId) == true)
                throw new Exception("Endpoint article id is missing.");

            return new Endpoint(label, articleId) { Content = article, Type = type };
        }

        private void WriteArticle(JsonWriter writer, Article article)
        {
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            var bytes = serializer.Serialize(article);
            using (var reader = new StreamReader(new MemoryStream(bytes), Encoding.UTF8))
            {
                writer.WriteRawValue(reader.ReadToEnd());
            }
        }

        private static readonly Dictionary<string, bool> _internal = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
        {
            {"__relationtype", true},
            {"__relationid", true}
        };

        protected override bool IsSytemProperty(string property)
        {
            if (base.IsSytemProperty(property) == true)
                return true;
            else
                return _internal.ContainsKey(property);
        }
    }

    
}
