using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class FindConnectedArticlesRequest : GetOperation<FindConnectedArticlesResponse>
    {
        public FindConnectedArticlesRequest() :
            this(AppacitiveContext.ApiKey, AppacitiveContext.SessionToken, AppacitiveContext.Environment, AppacitiveContext.UserToken, AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
        {
        }

        public FindConnectedArticlesRequest(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
        }

        public string Relation { get; set; }

        public string Type { get; set; }

        public string ArticleId { get; set; }

        public Article Article { get; set; }

        public bool ReturnEdge { get; set; }

        public string Label { get; set; }

        public string Query { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        protected override string GetUrl()
        {
            return Urls.For.FindConnectedArticles(this.Relation, this.Type, this.ArticleId, this.ReturnEdge, 
                this.Label, this.Query, this.PageNumber, this.PageSize, this.CurrentLocation, this.DebugEnabled, 
                this.Verbosity, this.Fields);
        }

        public override async Task<FindConnectedArticlesResponse> ExecuteAsync()
        {
            var response = await base.ExecuteAsync();
            // Set article in connection.
            if (response.Nodes != null && this.ReturnEdge == true )
            {
                foreach (var node in response.Nodes)
                {
                    var endpoint = string.IsNullOrWhiteSpace(node.Connection.Endpoints.EndpointA.ArticleId) ?
                        node.Connection.Endpoints.EndpointA :
                        node.Connection.Endpoints.EndpointB;
                    if (this.Article != null)
                        endpoint = new Endpoint(endpoint.Label, this.Article);
                    else
                        endpoint = new Endpoint(endpoint.Label, this.ArticleId);
                }
            }
            return response;
        }
    }
}
