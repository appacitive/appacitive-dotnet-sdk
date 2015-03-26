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
            AppContext.Initialize("appid", TestConfiguration.ApiKey, TestConfiguration.Environment,
                settings: new AppacitiveSettings
                {
                    HostName = "apis.appacitive.com/v1.0"
                });
            AppContext.Debug.ApiLogging.LogEverything();
            // Base class mappings
            AppContext.Types.MapObjectType<CustomUser>("user");
        }

        
    }
}
