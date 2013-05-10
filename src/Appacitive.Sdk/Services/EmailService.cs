using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    internal class EmailService : IEmailService
    {
        public static readonly IEmailService Instance = new EmailService();

        public async Task<SendEmailResponse> SendEmailAsync(SendEmailRequest request)
        {
            var bytes = await HttpOperation
                            .WithUrl(Urls.For.SendEmail(request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                            .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                            .WithEnvironment(request.Environment)
                            .WithUserToken(request.UserToken)
                            .PostAsyc(request.ToBytes());

            var response = SendEmailResponse.Parse(bytes);
            return response;
        }
    }
}
