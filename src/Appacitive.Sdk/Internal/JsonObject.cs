using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class JsonObject : IJsonObject
    {
        internal JsonObject( JObject json)
        {
            this.Inner = json;
        }

        public JsonObject(object content)
        {
            this.Inner = JObject.FromObject(content);
        }

        public JObject Inner { get; set; }

        public string AsString()
        {
            return this.Inner.ToString();
        }

        public byte[] AsBytes()
        {
            using (var stream = new MemoryStream())
            {
                using (var txtWriter = new StringWriter())
                {
                    using( var jsonWriter = new JsonTextWriter(txtWriter))
                    {
                        this.Inner.WriteTo(jsonWriter);
                        jsonWriter.Flush();
                        return stream.ToArray();
                    }
                }
            }
        }

        public override string ToString()
        {
            return AsString();
        }
    }
}
