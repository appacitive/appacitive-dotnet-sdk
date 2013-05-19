using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class PushFixture
    {
        [TestMethod]
        public async Task BroadcastPushAsyncTest()
        {
            string id = await PushNotifications
                .Broadcast("Push from .NET SDK")
                .WithBadge("+1")
                .WithData(new { field1 = "value1", field2 = "value2" })
                .SendAsync();
            Console.WriteLine("Send push notification with id {0}.", id);
        }
    }
}
