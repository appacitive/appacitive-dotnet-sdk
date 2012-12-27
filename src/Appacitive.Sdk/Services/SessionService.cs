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

        public CreateSessionResponse CreateSession(CreateSessionRequest request)
        {
            byte[] bytes = null;
            var timeTaken = Measure.TimeFor(() =>
                {
                    bytes = HttpClient
                        .WithUrl(Urls.For.CreateSession(request.DebugEnabled, request.Verbosity))
                        .Put(request.ToBytes());
                });
            var response = CreateSessionResponse.Parse(bytes);
            response.TimeTaken = timeTaken;
            return response;
        }

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
