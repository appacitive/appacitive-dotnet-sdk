using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class SessionServiceFixture
    {
        [TestMethod]
        public async Task CreateSessionAsyncTest()
        {
            var waitHandle = new ManualResetEvent(false);
            var service = ObjectFactory.Build<ISessionService>();
            CreateSessionResponse response = null;

            response = await service.CreateSessionAsync(new CreateSessionRequest() { ApiKey = TestConfiguration.ApiKey });
            waitHandle.Set();

            var isTimedOut = waitHandle.WaitOne(10000);
            Assert.IsTrue(isTimedOut, "Operation timed out.");
            Assert.IsNotNull(response, "Response cannot be null.");
            Assert.IsNotNull(response.Session, "Response.Session is null.");
            Assert.IsNotNull(response.Status, "Response.Status is null.");
            Console.WriteLine("Session token: {0}", response.Session.SessionKey);
            Console.WriteLine("Time taken: {0} seconds", response.TimeTaken);
        }
    }
}
