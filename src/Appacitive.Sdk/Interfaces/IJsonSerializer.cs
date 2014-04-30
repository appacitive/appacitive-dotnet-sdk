using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    /// <summary>
    /// Json serializer interface for handling serialization and deserialization for objects to and from json.
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// Returns the json serialized representation for the given object.
        /// </summary>
        /// <param name="o">The object to be serialized.</param>
        /// <returns>Json byte array.</returns>
        byte[] Serialize(object o);

        /// <summary>
        /// Deserializes the given json into an object instance of the given type.
        /// </summary>
        /// <param name="type">Object type.</param>
        /// <param name="stream">Json data stream.</param>
        /// <returns>The deserialized object.</returns>
        object Deserialize(Type type, byte[] stream);

        /// <summary>
        /// Deserializes the given json into an strongly typed object instance of the given type.
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="stream">Json data stream.</param>
        /// <returns>The deserialized object.</returns>
        T Deserialize<T>(byte[] stream);
    }

    public static class IJSonSerializerExtensions
    {
        public static bool TryDeserialize<T>(this IJsonSerializer serializer, string json, out T obj)
            where T : class
        {
            try
            {
                obj = serializer.Deserialize<T>(Encoding.UTF8.GetBytes(json));
                return true;
            }
            catch
            {
                obj = null;
                return false;
            }

        }

        public static bool TryDeserialize(this IJsonSerializer serializer, Type type, string json, out object obj)
        {
            try
            {
                obj = serializer.Deserialize(type, Encoding.UTF8.GetBytes(json));
                return true;
            }
            catch
            {
                obj = null;
                return false;
            }

        }


        public static string SerializeAsString(this IJsonSerializer serializer, object o)
        {
            if (o == null)
                return null;
            var bytes = serializer.Serialize(o);
            using (var memStream = new MemoryStream(bytes, false))
            {
                using (var streamReader = new StreamReader(memStream, Encoding.UTF8))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}
