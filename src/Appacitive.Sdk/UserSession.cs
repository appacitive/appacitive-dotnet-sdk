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
        public UserSession(User user, string token)
        {
            this.UserToken = token;
            this.LoggedInUser = user;
        }

        public User LoggedInUser { get; set; }

        public string UserToken { get; set; }

        public static async Task<bool> IsValidAsync(string authToken)
        {
            var userService = ObjectFactory.Build<IUserService>();
            var response = await userService.ValidateUserSessionAsync(
                new ValidateUserSessionRequest(AppacitiveContext.SessionToken, AppacitiveContext.Environment, authToken, 
                    AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
                );
            return response.Result == true;
        }

        public static async Task InvalidateAsync(string authToken)
        {
            var userService = ObjectFactory.Build<IUserService>();
            var response = await userService.InvalidateUserSessionAsync(
                new InvalidateUserSessionRequest(AppacitiveContext.SessionToken, AppacitiveContext.Environment, authToken,
                    AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
                );
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }
    }
}
