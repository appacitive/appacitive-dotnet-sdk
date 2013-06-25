﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using Appacitive.Sdk.Internal;

namespace Appacitive.Sdk.Services
{
    public abstract class EntityConverter : JsonConverter
    {
        private static readonly Dictionary<string, bool> _internal = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
        {
            {"__id", true},
            {"__revision", true},
            {"__createdby", true},
            {"__createdate", true},
            {"__lastmodifiedby", true},
            {"__utcdatecreated", true },
            {"__utclastupdateddate", true},
            {"__tags", true},
            {"__attributes", true}
        };

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var json = JObject.ReadFrom(reader) as JObject;
            if (json == null || json.Type == JTokenType.Null)
                return null;
            var entity = CreateEntity(json);
            return ReadJson(entity, objectType, json, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Entity entity = value as Entity;
            if (entity == null)
            {
                writer.WriteNull();
                return;
            }

            writer.StartObject();
            writer
                // Write id
                .WithWriter(w =>
                {
                    if (string.IsNullOrWhiteSpace(entity.Id) == false)
                        w.WriteProperty("__id", entity.Id);
                })
                // Write created by
                .WriteProperty("__createdby", entity.CreatedBy)
                // Write properties
                .WithWriter(w =>
                {

                    foreach (var property in entity.Properties)
                    {
                        if (property.Value is NullValue)
                        {
                            w.WritePropertyName(property.Key);
                            w.WriteNull();
                        }
                        else if (property.Value is MultiValue)
                        {
                            var collection = property.Value.GetValues<string>();
                            w.WritePropertyName(property.Key);
                            w.WriteStartArray();
                            foreach (var item in collection)
                                w.WriteValue(item);
                            w.WriteEndArray();
                        }
                        else
                            w.WriteProperty(property.Key, property.Value.GetValue<string>());
                    }
                })
                .WithWriter(w => WriteJson(entity, w, serializer))
                .WithWriter(w =>
                {
                    var attr = entity.Attributes.ToArray();
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
                .WithWriter(w =>
                {
                    if (entity.Tags.Count() > 0)
                        w.WriteArray("__tags", entity.Tags);
                });
            writer.EndObject();
        }

        protected abstract Entity CreateEntity(JObject json);

        protected virtual Entity ReadJson(Entity entity, Type objectType, JObject json, JsonSerializer serializer)
        {

            if (json == null || json.Type == JTokenType.Null)
                return null;
            JToken value;
            // Id
            if (json.TryGetValue("__id", out value) == true && value.Type != JTokenType.Null)
            {
                entity.Id = value.ToString();
            }
            // Revision
            if (json.TryGetValue("__revision", out value) == true && value.Type != JTokenType.Null)
                entity.Revision = int.Parse(value.ToString());
            // Created by
            if (json.TryGetValue("__createdby", out value) == true && value.Type != JTokenType.Null)
                entity.CreatedBy = value.ToString();
            // Create date
            if (json.TryGetValue("__utcdatecreated", out value) == true && value.Type != JTokenType.Null)
                entity.UtcCreateDate = (DateTime)value;
            // Last updated by
            if (json.TryGetValue("__lastmodifiedby", out value) == true && value.Type != JTokenType.Null)
                entity.LastUpdatedBy = value.ToString();
            // Last update date
            if (json.TryGetValue("__utclastupdateddate", out value) == true && value.Type != JTokenType.Null)
                entity.UtcLastUpdated = (DateTime)value;
            // tags
            if (json.TryGetValue("__tags", out value) == true && value.Type != JTokenType.Null)
                entity.AddTags(value.Values<string>(), true);

            // properties
            foreach (var property in json.Properties())
            {
                // Ignore system properties
                if (IsSytemProperty(property.Name) == true) continue;
                // Ignore objects
                else if (property.Value.Type == JTokenType.Object) continue;
                // Check for arrays
                else if (property.Value.Type == JTokenType.Array)
                {
                    entity.SetList<string>(property.Name, property.Value.Values<string>(), true);
                }
                // Set value of the property
                else if (property.Value.Type == JTokenType.Date)
                    entity.SetField(property.Name, ((DateTime)property.Value).ToString("o"), true);
                else
                    entity.SetField(property.Name, property.Value.Type == JTokenType.Null ? null : property.Value.ToString(), true);
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
                            entity.SetAttribute(property.Name, ((DateTime)property.Value).ToString("o"), true);
                        else
                            entity.SetAttribute(property.Name, property.Value.Type == JTokenType.Null ? null : property.Value.ToString(), true);
                    }
                }
            }
            return entity;
        }

        protected virtual void WriteJson(Entity entity, JsonWriter writer, JsonSerializer serializer)
        {
            return;
        }

        protected virtual bool IsSytemProperty(string property)
        {
            bool value;
            return _internal.TryGetValue(property, out value);
        }

    }

}
