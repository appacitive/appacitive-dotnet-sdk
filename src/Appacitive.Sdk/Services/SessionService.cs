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
            var timer = Stopwatch.StartNew();
            var bytes = await HttpClient
                .WithUrl(Urls.For.CreateSession(request.DebugEnabled, request.Verbosity))
                .PutAsyc(request.ToBytes());
            timer.Stop();
            var response = CreateSessionResponse.Parse(bytes);
            response.TimeTaken = (decimal)timer.ElapsedTicks / Stopwatch.Frequency;
            return response;
        }
    }
}
