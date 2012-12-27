using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public interface ISessionService
    {
        CreateSessionResponse CreateSession(CreateSessionRequest request);

        Task<CreateSessionResponse> CreateSessionAsync(CreateSessionRequest request);
    }

    
}
