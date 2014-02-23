using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class UserSession
    {   
        public UserSession(APUser user, string token)
        {
            this.UserToken = token;
            this.LoggedInUser = user;
        }

        public APUser LoggedInUser { get; set; }

        public string UserToken { get; set; }

        public static async Task<bool> IsValidAsync(string authToken)
        {
            var response = await new ValidateUserSessionRequest() { UserToken = authToken }.ExecuteAsync();
            return response.Result == true;
        }

        public static async Task InvalidateAsync(string authToken)
        {
            var response = await new InvalidateUserSessionRequest() { UserToken = authToken }.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }
    }
}
