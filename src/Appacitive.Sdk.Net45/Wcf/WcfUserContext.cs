using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Wcf
{
    public class WcfUserContext : IUserContext
    {
        public static readonly IUserContext Instance = new WcfUserContext();

        public void SetUserToken(string authToken)
        {
            // Remove existing
            var existing = OperationContext.Current.Extensions.Find<UserTokenExtension>();
            if( existing != null )
                OperationContext.Current.Extensions.Add(existing);
            // Add new
            OperationContext.Current.Extensions.Add(new UserTokenExtension(authToken));
        }

        public string GetUserToken()
        {
            var existing = OperationContext.Current.Extensions.Find<UserTokenExtension>();
            if (existing == null)
                return null;
            else return existing.UserToken;
        }
    }
}
