using Appacitive.Sdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class ServiceExceptionFactory : IExceptionFactory
    {
        public static readonly IExceptionFactory Instance = new ServiceExceptionFactory();

        public Exception CreateFault(Status status)
        {
            if (status == null || status.IsSuccessful == true)
                return null;
            return new AppacitiveException(status.Message)
            {
                Code = status.Code,
                ReferenceId = status.ReferenceId,
                FaultType = status.FaultType,
                AdditionalMessages = status.AdditionalMessages == null ? null : status.AdditionalMessages.ToArray()
            };
        }
    }
}
