using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Wcf
{
    public class WcfContextService : IContextService
    {
        public static readonly IContextService Instance = new WcfContextService();

        private OperationContext GetOperationContext()
        {
            try
            {
                var context = OperationContext.Current;
                if (context == null)
                    throw new AppacitiveRuntimeException("WCF operation context is not available.");
                return context;
            }
            catch( Exception ex )
            {
                throw new AppacitiveRuntimeException("Error aquiring wcf operation context.", ex);
            }
        }

        public APUser GetUser()
        {

            var extension = GetOperationContext().Extensions.Find<UserExtension>();
            if (extension == null)
                return null;
            else return extension.User;
        }

        public string GetUserToken()
        {
            var extension = GetOperationContext().Extensions.Find<UserTokenExtension>();
            if (extension == null)
                return null;
            else return extension.UserToken;
        }

        public APDevice GetDevice()
        {
            throw new AppacitiveRuntimeException("Device get is not supported for aspnet platform.");
        }

        public void SetDevice(APDevice device)
        {
            throw new AppacitiveRuntimeException("Device set is not supported for aspnet platform.");
        }

        public void SetUser(APUser user)
        {
            var context = GetOperationContext();
            // Remove the existing extension.
            var existing = context.Extensions.Find<UserExtension>();
            if (existing != null)
                context.Extensions.Remove(existing);
            // If user is not null, then update with latest content.
            if( user != null )
                context.Extensions.Add(new UserExtension(user));
            

        }

        public void SetUserToken(string value)
        {
            var context = GetOperationContext();
            // Remove the existing extension.
            var existing = context.Extensions.Find<UserTokenExtension>();
            if (existing != null)
                context.Extensions.Remove(existing);
            // If user is not null, then update with latest content.
            if (string.IsNullOrWhiteSpace(value) == false) 
                context.Extensions.Add(new UserTokenExtension(value));
            
        }
    }
}
