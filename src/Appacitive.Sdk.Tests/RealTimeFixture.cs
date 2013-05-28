using Appacitive.Sdk.Realtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class RealTimeFixture
    {
        [TestMethod]
        public async Task Run()
        {
            try
            {
                Console.WriteLine("test");
                await Messaging.JoinHubAsync("test");
                Console.WriteLine("Joined group");
                Console.WriteLine("Press any key to send message");
                await Messaging.SendMessageAsync(new { a = "test", b = "value" }, "test");
                Console.WriteLine("done");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
