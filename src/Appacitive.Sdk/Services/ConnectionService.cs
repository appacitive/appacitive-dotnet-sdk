using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace Appacitive.Sdk.Services
{
    public class ConnectionService : IConnectionService
    {
        public async Task<CreateConnectionResponse> CreateConnectionAsync(CreateConnectionRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpClient
                        .WithUrl(Urls.For.CreateConnection(request.Connection.Type, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
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
            bytes = await HttpClient
                        .WithUrl(Urls.For.GetConnection(request.Relation, request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
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
            bytes = await HttpClient
                        .WithUrl(Urls.For.GetConnection(request.Relation, request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                        .WithAppacitiveSession(request.SessionToken)
                        .WithEnvironment(request.Environment)
                        .WithUserToken(request.UserToken)
                        .DeleteAsync();
            var response = DeleteConnectionResponse.Parse(bytes);
            return response;
        }
    }
}
