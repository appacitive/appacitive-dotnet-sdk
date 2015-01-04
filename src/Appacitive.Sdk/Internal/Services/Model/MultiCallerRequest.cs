using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class MultiCallerRequest : PutOperation<MultiCallerResponse>
    {
        public MultiCallerRequest()
            : base()
        {
            this.ObjectSaveCommands = new List<object>();
            this.ConnectionSaveCommands = new List<object>();
            this.ObjectDeleteCommands = new List<object>();
            this.ConnectionDeleteCommands = new List<object>();
        }

        [JsonProperty("nodes")]
        public List<object> ObjectSaveCommands { get; private set; }

        [JsonProperty("edges")]
        public List<object> ConnectionSaveCommands { get; private set; }

        [JsonProperty("nodedeletions")]
        public List<object> ObjectDeleteCommands { get; private set; }

        [JsonProperty("edgedeletions")]
        public List<object> ConnectionDeleteCommands { get; private set; }

        protected override string GetUrl()
        {
            return Urls.For.MultiCaller(this.CurrentLocation, this.DebugEnabled, this.Verbosity);
        }
    }

    public class ObjectCreateCommand
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("object")]
        public APObject Object { get; set; }

    }

    public class ObjectUpdateCommand
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version", Required = Required.AllowNull)]
        public string Version { get; set; }

        [JsonProperty("object")]
        public IObjectUpdateRequest Object { get; set; }

    }

    public class ConnectionCreateCommand
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("connection")]
        public APConnection Connection { get; set; }
    }

    public class ConnectionUpdateCommand
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version", Required = Required.AllowNull)]
        public string Version { get; set; }

        [JsonProperty("connection")]
        public UpdateConnectionRequest Connection { get; set; }
    }

    public class ObjectDeleteCommand
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("deleteconnections")]
        public bool DeleteConnection { get; set; }
    }

    public class ConnectionDeleteCommand
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
