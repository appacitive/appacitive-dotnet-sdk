using Appacitive.Sdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.WinRT
{
    public class ExceptionFactory : IExceptionFactory
    {
        public static readonly IExceptionFactory Instance = new ExceptionFactory();

        public Exception CreateFault(Services.Status status)
        {
            if (status == null || status.IsSuccessful == true)
                return null;
            return new Appacitive.Sdk.WinRT.AppacitiveException(status.Message)
            {
                Code = status.Code,
                ReferenceId = status.ReferenceId,
                FaultType = status.FaultType,
                AdditionalMessages = status.AdditionalMessages == null ? null : status.AdditionalMessages.ToArray()
            };
        }
    }
}
