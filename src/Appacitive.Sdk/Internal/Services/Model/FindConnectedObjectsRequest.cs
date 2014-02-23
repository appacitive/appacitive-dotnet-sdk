using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class FindConnectedObjectsRequest : GetOperation<FindConnectedObjectsResponse>
    {
        public string Relation { get; set; }

        public string Type { get; set; }

        public string ObjectId { get; set; }

        public APObject Object { get; set; }

        public bool ReturnEdge { get; set; }

        public string Label { get; set; }

        public string Query { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string OrderBy { get; set; }

        public SortOrder SortOrder { get; set; }

        protected override string GetUrl()
        {
            return Urls.For.FindConnectedObjects(this.Relation, this.Type, this.ObjectId, this.ReturnEdge, 
                this.Label, this.Query, this.PageNumber, this.PageSize, this.OrderBy, this.SortOrder,
                this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }

        public override async Task<FindConnectedObjectsResponse> ExecuteAsync()
        {
            var response = await base.ExecuteAsync();
            // Set object in connection.
            if (response.Nodes != null && this.ReturnEdge == true )
            {
                foreach (var node in response.Nodes)
                {
                    var endpoint = string.IsNullOrWhiteSpace(node.Connection.Endpoints.EndpointA.ObjectId) ?
                        node.Connection.Endpoints.EndpointA :
                        node.Connection.Endpoints.EndpointB;
                    if (this.Object != null)
                        endpoint = new Endpoint(endpoint.Label, this.Object);
                    else
                        endpoint = new Endpoint(endpoint.Label, this.ObjectId);
                }
            }
            return response;
        }
    }
}
