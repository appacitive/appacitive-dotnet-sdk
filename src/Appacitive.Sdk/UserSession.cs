using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    /// <summary>
    /// Represents a user session generated when a user authenticates with the platform.
    /// </summary>
    public class UserSession
    {   
        /// <summary>
        /// Creates a new instance of UserSession with the given user and session token.
        /// </summary>
        /// <param name="user">The logged in user.</param>
        /// <param name="token">The authentication token returned by the platform.</param>
        public UserSession(APUser user, string token)
        {
            this.UserToken = token;
            this.LoggedInUser = user;
        }

        /// <summary>
        /// Gets the logged in user for this session object.
        /// </summary>
        public APUser LoggedInUser { get; set; }

        /// <summary>
        /// Gets the user session token for this session object.
        /// </summary>
        public string UserToken { get; set; }

        /// <summary>
        /// Gets whether the specified user session token is valid or not.
        /// </summary>
        /// <param name="authToken">User session token</param>
        /// <returns>True or false based on whether the given token is valid or not respecitively.</returns>
        public static async Task<bool> IsValidAsync(string authToken)
        {
            var response = await new ValidateUserSessionRequest() { UserToken = authToken }.ExecuteAsync();
            return response.Result == true;
        }

        /// <summary>
        /// Invalidates the given user session token on the backend server.
        /// </summary>
        /// <param name="authToken">User session token.</param>
        public static async Task InvalidateAsync(string authToken)
        {
            var response = await new InvalidateUserSessionRequest() { UserToken = authToken }.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }
    }
}
