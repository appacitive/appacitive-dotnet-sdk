using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class MultiCallerResponse : ApiResponse
    {
        [JsonProperty("nodes")]
        public List<NamedObject> AffectedObjects { get; set; }

        [JsonProperty("edges")]
        public List<NamedConnection> AffectedConnections { get; set; }

        [JsonProperty("nodedeletions")]
        public List<DeletedObject> DeletedObjects { get; set; }

        [JsonProperty("edgedeletions")]
        public List<DeletedConnection> DeletedConnections { get; set; }
    }


    public class NamedObject
    {
        public string Name { get; set; }

        public APObject Object { get; set; }
    }

    public class NamedConnection
    {
        public string Name { get; set; }

        public APConnection Connection { get; set; }
    }

    public class DeletedObject
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("deleteconnections")]
        public bool DeleteConnection { get; set; }
    }

    public class DeletedConnection
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
