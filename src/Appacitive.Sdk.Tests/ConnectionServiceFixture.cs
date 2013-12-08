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
            var response = await request.ExecuteAsync();
            ApiHelper.EnsureValidResponse(response);
            Assert.IsNotNull(response.Connection, "Connection in create connection response is null.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Connection.Id), "Connection id in response.connection is invalid.");
            var endpoints = response.Connection.Endpoints.ToArray();
            Assert.IsNotNull(endpoints[0], "Endpoint A is null.");
            Assert.IsNotNull(endpoints[1], "Endpoint B is null.");
            Assert.IsTrue(endpoints.Select(x => x.ObjectId).Intersect(new[] { obj1.Id, obj2.Id }).Count() == 2);
        }

        [TestMethod]
        public async Task CreateConnectionWithNewObjectsAsyncTest()
        {

            var obj1 = ObjectHelper.NewInstance();
            var obj2 = ObjectHelper.NewInstance();
            dynamic conn = new Connection("sibling", "object", obj1, "object", obj2);
            conn.field1 = Unique.String;
            conn.field2 = 123;

            var request = new CreateConnectionRequest() { Connection = conn };
            var response = await request.ExecuteAsync();
            ApiHelper.EnsureValidResponse(response);
            Assert.IsNotNull(response.Connection, "Connection in create connection response is null.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Connection.Id), "Connection id in response.connection is invalid.");
        }

        
    }
}
