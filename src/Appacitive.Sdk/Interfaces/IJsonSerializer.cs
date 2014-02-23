using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public interface IJsonSerializer
    {
        byte[] Serialize(object o);

        object Deserialize(Type type, byte[] stream);

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
