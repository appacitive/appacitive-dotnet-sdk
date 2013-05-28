using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public interface IConnectionService
    {
        Task<CreateConnectionResponse> CreateConnectionAsync(CreateConnectionRequest request);

        Task<GetConnectionResponse> GetConnectionAsync(GetConnectionRequest request);

        Task<UpdateConnectionResponse> UpdateConnectionAsync(UpdateConnectionRequest request);

        Task<GetConnectionByEndpointResponse> GetConnectionByEndpointAsync(GetConnectionByEndpointRequest request);

        Task<DeleteConnectionResponse> DeleteConnectionAsync(DeleteConnectionRequest request);

        Task<FindConnectedArticlesResponse> FindConnectedArticlesAsync(FindConnectedArticlesRequest request);

        Task<FindAllConectionsResponse> FindAllConnectionsAsync(FindAllConnectionsRequest request);

        Task<BulkDeleteConnectionResponse> BulkDeleteAsync(BulkDeleteConnectionRequest request); 
    }
}
