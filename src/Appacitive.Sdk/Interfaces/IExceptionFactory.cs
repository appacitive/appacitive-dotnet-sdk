using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Interfaces
{
    public interface IExceptionFactory
    {
        Exception CreateFault(Status status);
    }
}
