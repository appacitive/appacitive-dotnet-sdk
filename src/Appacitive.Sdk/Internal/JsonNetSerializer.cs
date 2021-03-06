﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Newtonsoft.Json;
using Appacitive.Sdk.Internal;

namespace Appacitive.Sdk
{
    public class JsonDotNetSerializer : IJsonSerializer
    {

        private static JsonSerializer _serializer = CreateNew();

        private static JsonSerializer CreateNew()
        {
            var serializer = new JsonSerializer();
            // TODO: Move this to objectfactory
            serializer.Converters.Add(new ObjectConverter());
            serializer.Converters.Add(new EmailConverter());
            serializer.Converters.Add(new UpdateListItemsRequestConverter());
            serializer.Converters.Add(new UpdateObjectRequestConverter());
            serializer.Converters.Add(new AtomicCountersRequestConverter());
            serializer.Converters.Add(new PushNotificationConverter());
            serializer.Converters.Add(new AuthenticateUserRequestConverter());
            serializer.Converters.Add(new ConnectionConverter());
            serializer.Converters.Add(new UpdateConnectionRequestConverter());
            serializer.Converters.Add(new GraphNodeConverter());
            serializer.Converters.Add(new GraphProjectResponseConverter());
            serializer.Converters.Add(new FindConnectedObjectsResponseConverter());
            return serializer;
        }

        public byte[] Serialize(object o)
        {
            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
                {
                    using (var jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        jsonWriter.Formatting = Formatting.Indented;
                        _serializer.Serialize(jsonWriter, o);
                    }
                }
                return stream.ToArray();
            }
        }

        public object Deserialize(Type type, byte[] stream)
        {
            using (var memStream = new MemoryStream(stream, false))
            {
                using (var streamReader = new StreamReader(memStream, Encoding.UTF8))
                {
                    using (var jsonReader = new JsonTextReader(streamReader))
                    {
                        return _serializer.Deserialize(jsonReader, type);
                    }
                }
            }
        }

        public T Deserialize<T>(byte[] stream)
        {
            using (var memStream = new MemoryStream(stream, false))
            {
                using (var streamReader = new StreamReader(memStream, Encoding.UTF8))
                {
                    using (var jsonReader = new JsonTextReader(streamReader))
                    {
                        return _serializer.Deserialize<T>(jsonReader);
                    }
                }
            }
        }
    }
}
