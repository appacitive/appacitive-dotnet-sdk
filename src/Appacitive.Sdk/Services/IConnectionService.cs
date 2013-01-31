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
    }
}
