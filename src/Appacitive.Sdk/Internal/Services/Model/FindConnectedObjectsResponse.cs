using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Appacitive.Sdk.Services
{
    public class FindConnectedObjectsResponse : ApiResponse
    {
        public PagingInfo PagingInfo { get; set; }

        public List<GraphNode> Nodes { get; set; }
    }
}
