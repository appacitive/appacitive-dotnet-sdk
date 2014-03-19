using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace Appacitive.Sdk.Services
{
    public class UpdateListItemsRequestConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(UpdateListItemsRequest) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var request = value as UpdateListItemsRequest;
            if (request == null)
            {
                writer.WriteNull();
                return;
            }
            /*
             "additems" : ["3","4","5"],
            "adduniqueitems" : ["1","6"],
            "removeitems": ["2","5"]
             */
            writer.WriteStartObject();
            writer.WritePropertyName(request.Property );
            writer.WriteStartObject();
            if (request.ItemsToAdd.Count > 0)
            {
                if (request.AddUniquely == true)
                    writer.WriteArray("adduniqueitems", request.ItemsToAdd);
                else
                    writer.WriteArray("additems", request.ItemsToAdd);
            }
            if (request.ItemsToRemove.Count > 0)
                writer.WriteArray("removeitems", request.ItemsToRemove);

            writer.WriteEndObject();
            writer.WriteEndObject();
        }
    }

}
