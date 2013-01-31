using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appacitive.Sdk.Tests
{
    internal class ApiHelper
    {
        public static void EnsureValidResponse(ApiResponse response, bool checkForSuccesss = true)
        {
            Assert.IsNotNull(response, "Api response is null.");
            Assert.IsNotNull(response.Status, "Response.Status is null.");
            if (checkForSuccesss == true)
            {
                if (response.Status.IsSuccessful == false)
                    Assert.Fail(response.Status.Message);
            }
        }
    }  
}
