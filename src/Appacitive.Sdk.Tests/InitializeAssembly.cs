using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Appacitive.Sdk.Net45;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class InitializeAssembly
    {
        [AssemblyInitialize]
        public static void Init(TestContext context)
        {
            App.Initialize(WindowsRT.Host, TestConfiguration.ApiKey, TestConfiguration.Environment,
                new AppacitiveSettings
                {
                    EnableRealTimeSupport = false
                });
            App.Debug.Out = Console.Out;
            App.Debug.IsEnabled = true;
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            App.Shutdown();
        }
    }
}
