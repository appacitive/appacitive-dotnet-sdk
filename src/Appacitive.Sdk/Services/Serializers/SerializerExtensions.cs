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
            if (values == null)
                return writer;
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

        public static JsonWriter WriteProperty(this JsonWriter writer, string property, int? value)
        {
            writer.WritePropertyName(property);
            writer.WriteValue(value);
            return writer;
        }

        public static JsonWriter WriteProperty(this JsonWriter writer, string property, bool? value)
        {
            writer.WritePropertyName(property);
            writer.WriteValue(value);
            return writer;
        }

        public static JsonWriter WriteProperty(this JsonWriter writer, string property, string value, bool ignoreIfNull = false)
        {
            if (value == null && ignoreIfNull == true)
                return writer ;
            writer.WritePropertyName(property);
            if (value == null )
                writer.WriteNull();
            else
                writer.WriteValue(value);
            return writer;
        }
    }
}
