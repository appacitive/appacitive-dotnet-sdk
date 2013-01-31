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
    public class ArticleConverter : EntityConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Article);
        }

        protected override Entity CreateEntity(JObject json)
        {
            JToken value;
            if (json.TryGetValue("__schematype", out value) == false || value.Type == JTokenType.Null)
                throw new Exception("Schema type missing.");
            var type = value.ToString();
            return new Article(type);
        } 

        protected override Entity ReadJson(Entity entity, Type objectType, JObject json)
        {
            if (json == null || json.Type == JTokenType.Null)
                return null;
            JToken value;
            var article = base.ReadJson(entity, objectType, json) as Article;
            if (article != null)
            {
                // Schema Id
                if (json.TryGetValue("__schemaid", out value) == true && value.Type != JTokenType.Null)
                    article.SchemaId = value.ToString();
                
            }
            return article;
        }
        
        protected override void WriteJson(Entity entity, JsonWriter writer)
        {
            if (entity == null)
                return;
            var article = entity as Article;
            if (article != null)
            {
                writer
                    .WriteProperty("__schematype", article.Type)
                    .WriteProperty("__schemaid", article.SchemaId);
            }
        }

        private static readonly Dictionary<string, bool> _internal = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
        {
            {"__schematype", true},
            {"__schemaid", true}
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
