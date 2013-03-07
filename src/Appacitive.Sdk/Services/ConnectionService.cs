using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Appacitive.Sdk.Services
{
    public class ConnectionService : IConnectionService
    {
        public static readonly IConnectionService Instance = new ConnectionService();

        public async Task<CreateConnectionResponse> CreateConnectionAsync(CreateConnectionRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                        .WithUrl(Urls.For.CreateConnection(request.Connection.Type, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                        .WithAppacitiveSession(request.SessionToken)
                        .WithEnvironment(request.Environment)
                        .WithUserToken(request.UserToken)
                        .PutAsyc(request.ToBytes());
            var response = CreateConnectionResponse.Parse(bytes);

            // Update the ids if any new articles were passed in the request.
            if (request.Connection.CreateEndpointA == true)
                request.Connection.EndpointA.Content.Id = response.Connection.EndpointA.ArticleId;
            if (request.Connection.CreateEndpointB == true)
                request.Connection.EndpointB.Content.Id = response.Connection.EndpointA.ArticleId;

            return response;
        }


        public async Task<GetConnectionResponse> GetConnectionAsync(GetConnectionRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                        .WithUrl(Urls.For.GetConnection(request.Relation, request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                        .WithAppacitiveSession(request.SessionToken)
                        .WithEnvironment(request.Environment)
                        .WithUserToken(request.UserToken)
                        .GetAsync();
            var response = GetConnectionResponse.Parse(bytes);
            return response;
        }

        public async Task<DeleteConnectionResponse> DeleteConnectionAsync(DeleteConnectionRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                        .WithUrl(Urls.For.GetConnection(request.Relation, request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                        .WithAppacitiveSession(request.SessionToken)
                        .WithEnvironment(request.Environment)
                        .WithUserToken(request.UserToken)
                        .DeleteAsync();
            var response = DeleteConnectionResponse.Parse(bytes);
            return response;
        }


        public async Task<FindConnectedArticlesResponse> FindConnectedArticlesAsync(FindConnectedArticlesRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                        .WithUrl(Urls.For.FindConnectedArticles(request.Relation, request.ArticleId, request.Query, request.PageNumber, request.PageSize, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                        .WithAppacitiveSession(request.SessionToken)
                        .WithEnvironment(request.Environment)
                        .WithUserToken(request.UserToken)
                        .GetAsync();
            var response = FindConnectedArticlesResponse.Parse(bytes);
            return response;
        }


        public async Task<GetConnectionByEndpointResponse> GetConnectionByEndpointAsync(GetConnectionByEndpointRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                        .WithUrl(Urls.For.GetConnectionByEndpointAsync(request.Relation, request.ArticleId1, request.ArticleId2, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                        .WithAppacitiveSession(request.SessionToken)
                        .WithEnvironment(request.Environment)
                        .WithUserToken(request.UserToken)
                        .GetAsync();
            var response = GetConnectionByEndpointResponse.Parse(bytes);
            return response;
        }


        public async Task<FindAllConectionsResponse> FindAllConnectionsAsync(FindAllConnectionsRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                        .WithUrl(Urls.For.FindAllConnectionsAsync(request.Type, request.Query, request.PageNumber, request.PageSize, request.OrderBy, request.SortOrder, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                        .WithAppacitiveSession(request.SessionToken)
                        .WithEnvironment(request.Environment)
                        .WithUserToken(request.UserToken)
                        .GetAsync();
            var response = FindAllConectionsResponse.Parse(bytes);
            return response;
        }
    }
}
