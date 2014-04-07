

﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//namespace Appacitive.Sdk.Services
//{
//    public class UserConverter : EntityConverter
//    {
//        public override bool CanConvert(Type objectType)
//        {
//            return typeof(APUser) == objectType;
//        }

//        protected override Entity CreateEntity(JObject json)
//        {
//            return new APUser();
//        }


//        protected override Entity ReadJson(Entity entity, Type objectType, JObject json, JsonSerializer serializer)
//        {
//            if (json == null || json.Type == JTokenType.Null)
//                return null;
//            JToken value;
//            var user = base.ReadJson(entity, objectType, json, serializer) as APUser;
//            if (user != null)
//                AclParser.ReadAcl(user, json, serializer);
//            if (user != null)
//            {
//                if (json.TryGetValue("__revision", out value) == true && value.Type != JTokenType.Null)
//                    user.Revision = int.Parse(value.ToString());
//            }
//            return user;
//        }

//        protected override void WriteJson(Entity entity, JsonWriter writer, JsonSerializer serializer)
//        {
//            if (entity == null)
//                return;
//            var user = entity as APUser;
//            //if (user != null)
//            //{
//            //    writer
//            //        .WriteProperty("__schemaid", user.SchemaId);
//            //}
//        }

//        private static readonly Dictionary<string, bool> _internal = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
//        {
//            {"__schematype", true},
//            {"__schemaid", true}
//        };

//        protected override bool IsSytemProperty(string property)
//        {
//            if (base.IsSytemProperty(property) == true)
//                return true;
//            else
//                return _internal.ContainsKey(property);

//        }
//    }
//}
