using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Appacitive.Sdk.Net45;
using System.Diagnostics;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class InitializeAssembly
    {
        [AssemblyInitialize]
        public static void Init(TestContext context)
        {
            App.Initialize(WindowsRT.Host, "napis.appacitive.com/v1.0", TestConfiguration.ApiKey, TestConfiguration.Environment,
            // App.Initialize(WindowsRT.Host, TestConfiguration.ApiKey, TestConfiguration.Environment,
                new AppacitiveSettings
                {
                    EnableRealTimeSupport = false
                });
            App.Debug.ApiLogging.LogEverything();
            // App.Debug.ApiLogging.LogSlowCalls(100);
            // App.Debug.ApiLogging.LogFailures();
            // App.Debug.ApiLogging.LogIf(x => x.Status.Code == "400");
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            App.Shutdown();
        }
    }
}
