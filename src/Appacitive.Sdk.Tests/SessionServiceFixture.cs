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
        // Configuration
        public static readonly string ApiKey = "up8+oWrzVTVIxl9ZiKtyamVKgBnV5xvmV95u1mEVRrM=";
        public static readonly Environment Env = Environment.Sandbox;

        [TestInitialize]
        public void Initialize()
        {
            App.Initialize(ApiKey, Env);
            var token = AppacitiveContext.SessionToken;
        }

        [TestMethod]
        public void CreateSessionTest()
        {
            var service = ObjectFactory.Build<ISessionService>();
            var response = service.CreateSession(new CreateSessionRequest() { ApiKey = ApiKey });
            Assert.IsNotNull(response, "Response cannot be null.");
            Assert.IsNotNull(response.Session, "Response.Session is null.");
            Assert.IsNotNull(response.Status, "Response.Status is null.");
            Console.WriteLine("Time taken: {0} seconds", response.TimeTaken);
        }

        [TestMethod]
        public void CreateSessionAsyncTest()
        {
            var waitHandle = new ManualResetEvent(false);
            var service = ObjectFactory.Build<ISessionService>();
            CreateSessionResponse response = null;
            Action action = async () =>
            {
                response = await service.CreateSessionAsync(new CreateSessionRequest() { ApiKey = ApiKey });
                waitHandle.Set();
            };
            action();
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
