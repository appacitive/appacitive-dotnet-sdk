using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Appacitive.Sdk.Connector
{

    public class StatusConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Status) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            /*
                "status": {
		        "code": "200",
		        "message": "Successful",
		        "faulttype": null,
		        "version": null,
		        "referenceid": "b0db3686-97e3-4517-8d29-ab46cdb70e42",
		        "additionalmessages": []
	        }
             */
            var json = JObject.ReadFrom(reader) as JObject;
            if (json == null || json.Type == JTokenType.Null)
                return null;
            var status = new Status();
            JToken value;
            // Code
            if (json.TryGetValue("code", out value) == true && value.Type != JTokenType.Null)
                status.Code = value.ToString();
            // Message
            if (json.TryGetValue("message", out value) == true && value.Type != JTokenType.Null)
                status.Message = value.ToString();
            // Fault type
            if (json.TryGetValue("faulttype", out value) == true && value.Type != JTokenType.Null)
                status.FaultType = value.ToString();
            // Reference id
            if (json.TryGetValue("referenceid", out value) == true && value.Type != JTokenType.Null)
                status.ReferenceId = value.ToString();
            // Additional messages
            if (json.TryGetValue("referenceid", out value) == true && value.Type != JTokenType.Null)
                status.AdditionalMessages.AddRange(value.Values<string>());
            return status;

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Cannot serialize Status.");
        }
    }

    public class ArticleConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Article);
        }

        /*
          {
			"__id": "11356486567592353",
			"__schematype": "person",
			"__schemaid": "11022991728181581",
			"__createdby": "Nikhil Prasad",
			"__lastmodifiedby": "Nikhil Prasad",
			"__tags": [],
			"__utcdatecreated": "2012-12-04T08:26:29.0000000",
			"__utclastupdateddate": "2012-12-04T08:26:29.0000000",
			"name": "Yetesh",
			"email": "ytokas@appacitive.com",
			"age": "30",
			"__attributes": {}
		},
         */

        private static readonly Dictionary<string, bool> _internal = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
        {
            {"__id", true},
            {"__createdby", true},
            {"__createdate", true},
            {"__lastmodifiedby", true},
            {"__utclastupdateddate", true},
            {"__tags", true},
            {"__schematype", true},
            {"__schemaid", true},
            {"__attributes", true}
        };
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var json = JObject.ReadFrom(reader) as JObject;
            if (json == null || json.Type == JTokenType.Null)
                return null;
            JToken value;
            if (json.TryGetValue("__schematype", out value) == false || value.Type == JTokenType.Null)
                throw new Exception("Schema type missing.");
            var type = value.ToString();
            var article = new Article(type);
            // Id
            if (json.TryGetValue("__id", out value) == true && value.Type != JTokenType.Null)
            {
                article.Id = value.ToString();
            }

            // Schema Id
            if (json.TryGetValue("__schemaid", out value) == true && value.Type != JTokenType.Null)
                article.SchemaId = value.ToString();
            // Created by
            if (json.TryGetValue("__createdby", out value) == true && value.Type != JTokenType.Null)
                article.CreatedBy = value.ToString();
            // Create date
            if (json.TryGetValue("__createdate", out value) == true && value.Type != JTokenType.Null)
                article.UtcCreateDate = DateTime.ParseExact(value.ToString(), "o", null);
            // Last updated by
            if (json.TryGetValue("__lastmodifiedby", out value) == true && value.Type != JTokenType.Null)
                article.LastUpdatedBy = value.ToString();
            // Last update date
            if (json.TryGetValue("__utclastupdateddate", out value) == true && value.Type != JTokenType.Null)
                article.UtcCreateDate = DateTime.ParseExact(value.ToString(), "o", null);
            // tags
            if (json.TryGetValue("__tags", out value) == true && value.Type != JTokenType.Null)
                article.Tags.AddRange(value.Values<string>());
            
            // properties
            foreach (var property in json.Properties())
            {
                // Ignore system properties
                if (IsSytemProperty(property.Name) == true) continue;
                // Ignore objects
                if (property.Value.Type == JTokenType.Object) continue;
                // Set value of the property
                if (property.Value.Type == JTokenType.Date)
                    article[property.Name] = ((DateTime)property.Value).ToString("o");
                else
                    article[property.Name] = property.Value.Type == JTokenType.Null ? null : property.Value.ToString();
            }

            // attributes
            JToken attributesJson;
            if (json.TryGetValue("__attributes", out attributesJson) == true)
            {
                var attr = attributesJson as JObject;
                if (attr != null)
                {
                    foreach (var property in attr.Properties())
                    {
                        // Ignore system properties
                        if (IsSytemProperty(property.Name) == true) continue;
                        // Ignore objects
                        if (property.Value.Type == JTokenType.Object) continue;
                        // Set value of the property
                        if (property.Value.Type == JTokenType.Date)
                            article["@" + property.Name] = ((DateTime)property.Value).ToString("o");
                        else
                            article["@" + property.Name] = property.Value.Type == JTokenType.Null ? null : property.Value.ToString();
                    }
                }
            }
            return article;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Article article = value as Article;
            if( article == null )
            {
                writer.WriteNull();
                return;
            }

            writer
                .StartObject()
                .WriteProperty("__id", article.Id)
                .WriteProperty("__createdby", article.CreatedBy)
                .WriteProperty("__lastmodifiedby", article.LastUpdatedBy)
                .WithWriter( w => 
                    {
                        // Write properties
                        foreach (var property in article.Properties)
                            w.WriteProperty(property.Key, property.Value);
                    })
                .WithWriter( w => 
                    {
                        var attr = article.Attributes.ToArray();
                        if (attr.Length > 0)
                        {
                            w.WriteProperty("__attributes")
                             .StartObject();
                            // Write attributes
                            for (int i = 0; i < attr.Length; i++)
                                w.WriteProperty(attr[i].Key, attr[i].Value);
                            w.EndObject();
                        }
                            
                    })
                .WriteArray("__tags", article.Tags.Count > 0 ? article.Tags : null )
                .EndObject();
        }

        private bool IsSytemProperty(string property)
        {
            bool value;
            return _internal.TryGetValue(property, out value);
        }
    }

    internal static class SerializerExtensions
    {

        public static JsonWriter WithWriter(this JsonWriter writer, Action<JsonWriter> action)
        {
            action(writer);
            return writer;
        }

        public static JsonWriter StartObject( this JsonWriter writer )
        {
            writer.WriteStartObject();
            return writer;
        }

        public static JsonWriter EndObject(this JsonWriter writer)
        {
            writer.WriteEndObject();
            return writer;
        }

        public static JsonWriter WriteArray(this JsonWriter writer, string property, IEnumerable<string> values)
        {
            writer.WritePropertyName(property);
            if (values == null )
                writer.WriteNull();
            else
            {
                writer.WriteStartArray();
                foreach (var value in values)
                    writer.WriteValue(value);
                writer.WriteEndArray();
            }
            return writer;
        }

        public static JsonWriter WriteProperty(this JsonWriter writer, string property)
        {
            writer.WritePropertyName(property);
            return writer;
        }

        public static JsonWriter WriteProperty(this JsonWriter writer, string property, bool? value)
        {
            writer.WritePropertyName(property);
            writer.WriteValue(value);
            return writer;
        }

        public static JsonWriter WriteProperty(this JsonWriter writer, string property, string value )
        {
            writer.WritePropertyName(property);
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(value);
            return writer;
        }
    }
}
