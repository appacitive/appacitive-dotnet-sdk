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
        public void CreateConnectionTest()
        {
            var obj1 = ObjectHelper.CreateNew();
            var obj2 = ObjectHelper.CreateNew();
            dynamic conn = new Connection("sibling", "object", obj1.Id, "object", obj2.Id);
            conn.field1 = Unique.String;
            conn.field2 = 123;

            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
                {
                    try
                    {
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
                    catch (Exception ex)
                    {
                        fault = ex;
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                };
            action();
            waitHandle.WaitOne();
            if (fault != null)
                throw fault;
        }

        [TestMethod]
        public void CreateConnectionWithNewArticlesTest()
        {
            
            var obj1 = ObjectHelper.NewInstance();
            var obj2 = ObjectHelper.NewInstance();
            dynamic conn = new Connection("sibling", "object", obj1, "object", obj2);
            conn.field1 = Unique.String;
            conn.field2 = 123;
            
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
            {
                try
                {
                    var request = new CreateConnectionRequest() { Connection = conn };
                    IConnectionService connService = new ConnectionService();
                    var response = await connService.CreateConnectionAsync(request);
                    ApiHelper.EnsureValidResponse(response);
                    Assert.IsNotNull(response.Connection, "Connection in create connection response is null.");
                    Assert.IsFalse(string.IsNullOrWhiteSpace(response.Connection.Id), "Connection id in response.connection is invalid.");
                    Assert.IsNotNull(response.Connection.EndpointA, "Endpoint A is null.");
                    Assert.IsNotNull(response.Connection.EndpointA, "Endpoint B is null.");
                }
                catch (Exception ex)
                {
                    fault = ex;
                }
                finally
                {
                    waitHandle.Set();
                }
            };
            action();
            waitHandle.WaitOne();
            if (fault != null)
                throw fault;
        }
    }
}
