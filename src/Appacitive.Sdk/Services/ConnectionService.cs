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
            return response;
        }
    }
}
