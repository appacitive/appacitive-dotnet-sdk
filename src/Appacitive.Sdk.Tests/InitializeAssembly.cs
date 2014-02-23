using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Appacitive.Sdk;
using System.Diagnostics;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class InitializeAssembly
    {
        [AssemblyInitialize]
        public static void Init(TestContext context)
        {
            App.Initialize(Platforms.Net, "appid", TestConfiguration.ApiKey, TestConfiguration.Environment);
            App.Debug.ApiLogging.LogEverything();
            // App.Debug.ApiLogging.LogSlowCalls(100);
            // App.Debug.ApiLogging.LogFailures();
            // App.Debug.ApiLogging.LogIf(x => x.Status.Code == "400");
        }

        
    }
}
