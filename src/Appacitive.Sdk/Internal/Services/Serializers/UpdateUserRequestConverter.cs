//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Appacitive.Sdk.Services;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System.Collections;

//namespace Appacitive.Sdk.Services
//{
//    public class UpdateUserRequestConverter : JsonConverter
//    {
//        public override bool CanConvert(Type objectType)
//        {
//            return typeof(UpdateUserRequest) == objectType;
//        }

//        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//        {
//            throw new NotImplementedException();
//        }

//        private void WriteProperty(JsonWriter writer, string key, object property)
//        {
//            writer.WritePropertyName(key);
//            if (property != null && property is string == false && property is IEnumerable)
//            {
//                var enumerable = property as IEnumerable;
//                writer.WriteStartArray();
//                foreach (var item in enumerable)
//                    writer.WriteValue(item.ToString());
//                writer.WriteEndArray();
//            }
//            else
//            {
//                if (property == null)
//                    writer.WriteNull();
//                else
//                    writer.WriteValue(property.ToString());
//            }

//        }

//        private void WriteAcls(JsonWriter writer, List<Claim> allowed, List<Claim> denied, List<ResetRequest> reset)
//        {
//            var map = new Dictionary<SidTypeKey, ClaimGroup>();
//            allowed.For(x =>
//            {
//                ClaimGroup group;
//                var key = new SidTypeKey { Sid = x.Sid, Type = x.ClaimType };
//                if (map.TryGetValue(key, out group) == false)
//                    group = new ClaimGroup { Sid = x.Sid, Type = x.ClaimType };
//                group.Allowed.Add(x.AccessType);
//                map[key] = group;
//            });
//            denied.For(x =>
//            {
//                ClaimGroup group;
//                var key = new SidTypeKey { Sid = x.Sid, Type = x.ClaimType };
//                if (map.TryGetValue(key, out group) == false)
//                    group = new ClaimGroup { Sid = x.Sid, Type = x.ClaimType };
//                group.Denied.Add(x.AccessType);
//                map[key] = group;
//            });
//            reset.For(x =>
//            {
//                ClaimGroup group;
//                var key = new SidTypeKey { Sid = x.Sid, Type = x.Type };
//                if (map.TryGetValue(key, out group) == false)
//                    group = new ClaimGroup { Sid = x.Sid, Type = x.Type };
//                group.Reset.Add(x.Access);
//                map[key] = group;
//            });

//            writer.WriteStartArray();
//            map.Values.For(x =>
//            {
//                writer.WriteStartObject();
//                writer.WriteProperty("sid", x.Sid);
//                writer.WriteProperty("type", x.Type.ToString().ToLower());
//                if (x.Allowed.Count > 0)
//                    writer.WriteArray("allow", x.Allowed.Select(y => y.ToString().ToLower()));
//                if (x.Denied.Count > 0)
//                    writer.WriteArray("deny", x.Denied.Select(y => y.ToString().ToLower()));
//                if (x.Reset.Count > 0)
//                    writer.WriteArray("dontcare", x.Reset.Select(y => y.ToString().ToLower()));
//                writer.WriteEndObject();
//            });
//            writer.WriteEndArray();
//        }

//        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//        {
//            var request = value as UpdateUserRequest;
//            if (request == null)
//            {
//                writer.WriteNull();
//                return;
//            }
            
//            writer
//                .StartObject()
//                // Write properties
//                .WithWriter(w => request.PropertyUpdates.For(p => WriteProperty(w, p.Key, p.Value)))
//                // Write atttributes
//                .WithWriter(w =>
//                {
//                    if (request.AttributeUpdates.Count > 0)
//                    {
//                        w.WriteProperty("__attributes")
//                         .StartObject()
//                         .WithWriter(w2 => request.AttributeUpdates.For(a => w2.WriteProperty(a.Key, a.Value)))
//                         .EndObject();
//                    }
//                })
//                // Write acls
//                .WithWriter(w =>
//                {
//                    if (request.AllowClaims.Count == 0 && request.DenyClaims.Count == 0) return;
//                    w.WritePropertyName("__acls");
//                    WriteAcls(w, request.AllowClaims, request.DenyClaims, request.ResetClaims);
//                })
//                // Write add tags
//                .WithWriter(w =>
//                {
//                    if (request.AddedTags.Count > 0)
//                        w.WriteArray("__addtags", request.AddedTags);
//                })
//                // Write remove tags
//                .WithWriter(w =>
//                {
//                    if (request.RemovedTags.Count > 0)
//                        w.WriteArray("__removetags", request.RemovedTags);
//                })
//                .EndObject();
//        }
//    }
//}
