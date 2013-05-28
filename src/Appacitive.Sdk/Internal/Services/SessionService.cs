using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class SessionService : ISessionService
    {
        internal static SessionService Instance = new SessionService();

        public async Task<CreateSessionResponse> CreateSessionAsync(CreateSessionRequest request)
        {
            var bytes = await HttpOperation
                .WithUrl(Urls.For.CreateSession(request.DebugEnabled, request.Verbosity))
                .PutAsyc(request.ToBytes());
            var response = CreateSessionResponse.Parse(bytes);
            return response;
        }
    }
}
