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

namespace Appacitive.Sdk.Services
{
    public class ObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Is<APObject>();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Read the type of the object and create  a new instance.
            // Invoke a helper to populate the common fields for the entity
            // Parse the rest
            var json = JObject.ReadFrom(reader) as JObject;
            if (json == null || json.Type == JTokenType.Null)
                return null;
            var instance = BuildNewInstance(json, objectType);
            EntityParser.ReadJson(instance, json, serializer);
            // The only field only available in ap object is acl.
            AclParser.ReadAcl(instance, json, serializer);
            // Read any type specific fields
            if (instance is APUser)
                UserParser.ReadJson(instance as APUser, json);
            return instance;
        }

        private APObject BuildNewInstance(JObject json, Type objectType)
        {
            if (objectType == typeof(APDevice))
            {
                JToken value;
                string type = string.Empty;
                if (json.TryGetValue("devicetype", out value) == true && value.Type != JTokenType.Null)
                    type = value.ToString();
                else throw new Exception("DeviceType not present in response.");
                return new APDevice(SupportedDevices.ResolveDeviceType(type));
            }
            else
            {
                string type;
                if (objectType.Is<APUser>() == true)
                    type = "user";
                else 
                    type = GetType(json);
                var mappedType = InternalApp.Types.Mapping.GetMappedObjectType(type);
                if (mappedType == null)
                    return new APObject(type);
                else
                    return Activator.CreateInstance(mappedType) as APObject;
            }
        }


        private string GetType(JObject json)
        {
            JToken value;
            if (json.TryGetValue("__type", out value) == false || value.Type == JTokenType.Null)
                throw new Exception("Object type is missing.");
            var type = value.ToString();
            return type;
        }
        

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Manage the open and closing of the object.
            // Write APObject specific content (e.g., acls)
            // Call appropriate helpers to write common stuff.
            APObject obj = value as APObject;
            if (obj == null)
            {
                writer.WriteNull();
                return;
            }
            writer.StartObject();
            EntityParser.WriteJson(writer, obj, serializer);
            AclParser.WriteAcl(writer, obj, serializer);
            writer.WriteEndObject();
        }
    }

    internal class UserParser
    {
        internal static void ReadJson(APUser user, JObject json)
        {
            /*
            "__groups": [
			{
				"groupid": "12345",
				"name": "group name"
			}
            */
            JToken value;
            if (json.TryGetValue("__groups", out value) == false) return;
            if (value.Type != JTokenType.Array) return;
            foreach (JObject groupJson in value.Values<JObject>()) 
            {
                var groupInfo = ParseGroupInfo(groupJson);
                if (groupInfo != null)
                    user.Groups.Add(groupInfo);
            }

        }

        private static GroupInfo ParseGroupInfo(JObject json)
        {
            JToken id, name;
            if (json.TryGetValue("groupid", out id) == false)
                return null;
            if (json.TryGetValue("name", out name) == false)
                return null;
            return new GroupInfo(id.ToString(), name.ToString());
        }
    }

    internal class AclParser
    {
        public static void ReadAcl(APObject obj, JObject json, JsonSerializer serializer)
        {
            JToken value;
            if (json.TryGetValue("__acls", out value) == false)
                return;
            var array = value as JArray;
            if (array == null || array.Count == 0) return;
            var items = array.Values<JObject>();
            foreach (var item in items)
            {
                ParseClaims(obj.Acl, item);
            }
        }

        private static void ParseClaims(Acl acl, JObject item)
        {
            var sid = item.Property("sid").Value.ToString();
            var type = item.Property("type").Value.ToString();
            List<Access> allowed = new List<Access>();
            List<Access> denied = new List<Access>();
            var allowProperty = item.Property("allow");
            if (allowProperty != null)
            {
                allowProperty
                    .Values()
                    .Select(x => (Access)Enum.Parse(typeof(Access), x.ToString(), true))
                    .For(x => allowed.Add(x));
            }
            var denyProperty = item.Property("deny");
            if (denyProperty != null)
            {
                denyProperty
                    .Values()
                    .Select(x => (Access)Enum.Parse(typeof(Access), x.ToString(), true))
                    .For(x => denied.Add(x));
            }
            ClaimType claimType = (ClaimType)Enum.Parse(typeof(ClaimType), type, true);
            List<Claim> claims = new List<Claim>();
            allowed.For(x => claims.Add(new Claim(Permission.Allow, x, claimType, sid)));
            denied.For(x => claims.Add(new Claim(Permission.Deny, x, claimType, sid)));
            acl.SetInternal(claims);
        }

        public static void WriteAcl(JsonWriter writer, APObject obj, JsonSerializer serializer)
        {
            var acl = obj.Acl;
            var allowed = acl.Allowed.ToList();
            var denied = acl.Denied.ToList();
            var inherit = acl.Reset.ToList();
            if (allowed.Count == 0 && denied.Count == 0 && inherit.Count == 0)
                return;
            writer.WritePropertyName("__acls");
            WriteAcls(writer, allowed, denied, inherit);
        }

        private static void WriteAcls(JsonWriter writer, List<Claim> allowed, List<Claim> denied, List<ResetRequest> reset)
        {
            var map = new Dictionary<SidTypeKey, ClaimGroup>();
            allowed.For(x =>
            {
                ClaimGroup group;
                var key = new SidTypeKey { Sid = x.Sid, Type = x.ClaimType };
                if (map.TryGetValue(key, out group) == false)
                    group = new ClaimGroup { Sid = x.Sid, Type = x.ClaimType };
                group.Allowed.Add(x.AccessType);
            });
            denied.For(x =>
            {
                ClaimGroup group;
                var key = new SidTypeKey { Sid = x.Sid, Type = x.ClaimType };
                if (map.TryGetValue(key, out group) == false)
                    group = new ClaimGroup { Sid = x.Sid, Type = x.ClaimType };
                group.Denied.Add(x.AccessType);
            });
            reset.For(x =>
            {
                ClaimGroup group;
                var key = new SidTypeKey { Sid = x.Sid, Type = x.Type };
                if (map.TryGetValue(key, out group) == false)
                    group = new ClaimGroup { Sid = x.Sid, Type = x.Type };
                group.Reset.Add(x.Access);
            });

            writer.WriteStartArray();
            map.Values.For(x =>
            {
                writer.WriteStartObject();
                writer.WriteProperty("sid", x.Sid);
                writer.WriteProperty("type", x.Type.ToString().ToLower());
                if (x.Allowed.Count > 0)
                    writer.WriteArray("allow", x.Allowed.Select(y => y.ToString().ToLower()));
                if (x.Denied.Count > 0)
                    writer.WriteArray("deny", x.Denied.Select(y => y.ToString().ToLower()));
                if (x.Reset.Count > 0)
                    writer.WriteArray("dontcare", x.Reset.Select(y => y.ToString().ToLower()));
                writer.WriteEndObject();
            });
            writer.WriteEndArray();
        }
    }

    internal static class EntityParser
    {
        public static void ReadJson(Entity entity, JObject json, JsonSerializer serializer)
        {
            if (json == null || json.Type == JTokenType.Null)
                return;
            // Parse system propertues
            ParseSystemProperties(entity, json, serializer);
            // Parse user defined properties
            ParseUserDefinedProperties(entity, json, serializer);
            // Parse attributes
            ParseAttributes(entity, json, serializer);
        }

        private static void ParseAttributes(Entity entity, JObject json, JsonSerializer serializer)
        {
            JToken attributesJson;
            if (json.TryGetValue("__attributes", out attributesJson) == false)
                return;
            var attr = attributesJson as JObject;
            if (attr == null) return;

            var attributes = attr.Properties();
            foreach (var attribute in attributes)
            {
                if (attribute.Value.Type == JTokenType.Object) continue;
                // Set value of the property
                if (attribute.Value.Type == JTokenType.Date)
                    entity.SetAttribute(attribute.Name, ((DateTime)attribute.Value).ToString("o"), true);
                else
                    entity.SetAttribute(attribute.Name, attribute.Value.Type == JTokenType.Null ? null : attribute.Value.ToString(), true);
            }
        }

        private static void ParseUserDefinedProperties(Entity entity, JObject json, JsonSerializer serializer)
        {
            // properties
            var properties = json.Properties().Where(x => IsUserDefinedProperty(x) == true);
            foreach (var property in properties)
            {
                // Check for arrays
                if (property.Value.Type == JTokenType.Array)
                {
                    entity.SetList<string>(property.Name, property.Value.Values<string>(), true);
                }
                // Set value of the property
                else if (property.Value.Type == JTokenType.Date)
                    entity.SetField(property.Name, ((DateTime)property.Value).ToString("o"), true);
                else
                    entity.SetField(property.Name, property.Value.Type == JTokenType.Null ? null : property.Value.ToString(), true);
            }
        }

        private static void ParseSystemProperties(Entity entity, JObject json, JsonSerializer serializer)
        {
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
                entity.CreatedAt = ((DateTime)value).ToLocalTime();
            // Last updated by
            if (json.TryGetValue("__lastmodifiedby", out value) == true && value.Type != JTokenType.Null)
                entity.LastUpdatedBy = value.ToString();
            // Last update date
            if (json.TryGetValue("__utclastupdateddate", out value) == true && value.Type != JTokenType.Null)
                entity.LastUpdatedAt = ((DateTime)value).ToLocalTime();
            // tags
            if (json.TryGetValue("__tags", out value) == true && value.Type != JTokenType.Null)
                entity.AddTags(value.Values<string>(), true);
        }

        private static bool IsUserDefinedProperty(JProperty property)
        {
            // Ignore system types
            if (property.Name.StartsWith("_") == true)
                return false;
            if (property.Value.Type == JTokenType.Object)
                return false;
            return true;
        }

        public static void WriteJson(JsonWriter writer, Entity entity, JsonSerializer serializer)
        {
            // Write id
            if (string.IsNullOrWhiteSpace(entity.Id) == false)
                writer.WriteProperty("__id", entity.Id);
            // Write properties.
            WriteProperties(writer, entity, serializer);
            // Write attributes
            WriteAttributes(writer, entity, serializer);
            // Write Tags
            WriteTags(writer, entity, serializer);
        }

        private static void WriteTags(JsonWriter writer, Entity entity, JsonSerializer serializer)
        {
            if (entity.Tags.Count() > 0)
                writer.WriteArray("__tags", entity.Tags);
        }



        private static void WriteAttributes(JsonWriter writer, Entity entity, JsonSerializer serializer)
        {
            var attr = entity.Attributes.ToArray();
            if (attr.Length > 0)
            {
                writer.WriteProperty("__attributes");
                writer.StartObject();
                // Write attributes
                for (int i = 0; i < attr.Length; i++)
                    writer.WriteProperty(attr[i].Key, attr[i].Value);
                writer.EndObject();
            }
        }

        private static void WriteProperties(JsonWriter writer, Entity entity, JsonSerializer serializer)
        {
            foreach (var property in entity.Properties)
            {
                if (property.Value is NullValue)
                {
                    writer.WritePropertyName(property.Key);
                    writer.WriteNull();
                }
                else if (property.Value is MultiValue)
                {
                    var collection = property.Value.GetValues<string>();
                    writer.WriteArray(property.Key, collection);
                }
                else
                    writer.WriteProperty(property.Key, property.Value.GetValue<string>());
            }
        }
    }
}
