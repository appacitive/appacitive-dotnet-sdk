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
            App.Initialize("appid", TestConfiguration.ApiKey, TestConfiguration.Environment);
            App.Debug.ApiLogging.LogEverything();
            // Base class mappings
            App.Types.MapObjectType<CustomUser>("user");
        }

        
    }
}
