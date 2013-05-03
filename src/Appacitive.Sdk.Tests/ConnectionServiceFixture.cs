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
    public class ConnectionServiceFixture
    {
        [TestMethod]
        public async Task CreateConnectionTest()
        {
            var obj1 = await ObjectHelper.CreateNewAsync();
            var obj2 = await ObjectHelper.CreateNewAsync();
            dynamic conn = new Connection("sibling", "object", obj1.Id, "object", obj2.Id);
            conn.field1 = Unique.String;
            conn.field2 = 123;

            var request = new CreateConnectionRequest() { Connection = conn };
            IConnectionService connService = new ConnectionService();
            var response = await connService.CreateConnectionAsync(request);
            ApiHelper.EnsureValidResponse(response);
            Assert.IsNotNull(response.Connection, "Connection in create connection response is null.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Connection.Id), "Connection id in response.connection is invalid.");
            Assert.IsNotNull(response.Connection.EndpointA, "Endpoint A is null.");
            Assert.IsNotNull(response.Connection.EndpointA, "Endpoint B is null.");
            Assert.IsTrue(response.Connection.EndpointA.ArticleId == obj1.Id, "Endpoint A article id does not match.");
            Assert.IsTrue(response.Connection.EndpointB.ArticleId == obj2.Id, "Endpoint A article id does not match.");
        }

        [TestMethod]
        public async Task CreateConnectionWithNewArticlesAsyncTest()
        {

            var obj1 = ObjectHelper.NewInstance();
            var obj2 = ObjectHelper.NewInstance();
            dynamic conn = new Connection("sibling", "object", obj1, "object", obj2);
            conn.field1 = Unique.String;
            conn.field2 = 123;

            var request = new CreateConnectionRequest() { Connection = conn };
            IConnectionService connService = new ConnectionService();
            var response = await connService.CreateConnectionAsync(request);
            ApiHelper.EnsureValidResponse(response);
            Assert.IsNotNull(response.Connection, "Connection in create connection response is null.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Connection.Id), "Connection id in response.connection is invalid.");
            Assert.IsNotNull(response.Connection.EndpointA, "Endpoint A is null.");
            Assert.IsNotNull(response.Connection.EndpointA, "Endpoint B is null.");
        }

        [TestMethod]
        public async Task GetConnectedArticlesTest3()
        {
            App.Initialize(WinRT.WindowsHost.Instance, "+HfTOp2nF8TnkyZVBblkTBLm6Cz6zIfKYdXBhV6Aag4=", Environment.Live);
            IConnectionService connService = new ConnectionService();
            var request = new FindConnectedArticlesRequest()
                {
                    ArticleId = "22239781196006111",
                    Relation = "userteam_player"
                };
            var response = await connService.FindConnectedArticlesAsync(request);
            int index = 1;
            response.Connections.ForEach(c =>
                {
                    Console.WriteLine("{2}) A = {0}, B = {1}, {3}", c.EndpointA.ArticleId, c.EndpointB.ArticleId, index++, c.EndpointB.Content["firstname"]);
                });
            
        }
    }
}
