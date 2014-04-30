using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    /// <summary>
    /// Represents the application specific state used by the SDK.
    /// </summary>
    public interface IApplicationState
    {
        /// <summary>
        /// Gets the current logged in user.
        /// </summary>
        /// <returns>APUser representing the logged in user.</returns>
        APUser GetUser();

        /// <summary>
        /// Gets the current user session token.
        /// </summary>
        string GetUserToken();

        /// <summary>
        /// Sets the currently logged in user.
        /// </summary>
        /// <param name="user">APUser representing the logged in user.</param>
        void SetUser(APUser user);

        /// <summary>
        /// Sets the current user session token.
        /// </summary>
        /// <param name="value">User session token.</param>
        void SetUserToken(string value);
    }
}

