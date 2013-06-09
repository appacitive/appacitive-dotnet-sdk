using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class ConnectionFixture
    {
        [TestMethod]
        public async Task ConnectionsTest()
        {
            var parent = ObjectHelper.NewInstance();
            parent["stringfield"] = "parent";
            var child = ObjectHelper.NewInstance();
            child["stringfield"] = "child";
            var conn = Connection.New("link").FromNewArticle("parent", parent).ToNewArticle("child", child);
            await conn.SaveAsync();

            var parent2 = await conn.Endpoints["parent"].GetArticleAsync();
            var child2 = await conn.Endpoints["child"].GetArticleAsync();
            Assert.IsTrue(parent2 != null && child2 != null);
            Assert.IsTrue(parent2.Get<string>("stringfield") == "parent");
            Assert.IsTrue(child2.Get<string>("stringfield") == "child");

            // Swap and test
            parent = ObjectHelper.NewInstance();
            parent["stringfield"] = "parent";
            child = ObjectHelper.NewInstance();
            child["stringfield"] = "child";
            conn = Connection.New("link").FromNewArticle("child", child).ToNewArticle("parent", parent);
            await conn.SaveAsync();

            parent2 = await conn.Endpoints["parent"].GetArticleAsync();
            child2 = await conn.Endpoints["child"].GetArticleAsync();
            Assert.IsTrue(parent2 != null && child2 != null);
            Assert.IsTrue(parent2.Get<string>("stringfield") == "parent");
            Assert.IsTrue(child2.Get<string>("stringfield") == "child");
        }
        
        [TestMethod]
        public async Task GetEndpointContentTest()
        {
            var existing = await ObjectHelper.CreateNewAsync();
            var newObj = ObjectHelper.NewInstance();
            var conn = Connection
                .New("link")
                .FromExistingArticle("parent", existing.Id)
                .ToNewArticle("child", newObj);
            await conn.SaveAsync();

            Assert.IsTrue(conn.GetEndpointId("parent") == existing.Id);
            Assert.IsTrue( string.IsNullOrWhiteSpace(conn.GetEndpointId("child")) == false );
            
            // Get endpoints
            var child = await conn.GetEndpointArticleAsync("child");
            var parent = await conn.GetEndpointArticleAsync("parent");
            Assert.IsNotNull(child);
            Assert.IsNotNull(parent);
            Assert.IsTrue(parent.Id == existing.Id);
        }

        [TestMethod]
        public async Task CreateConnectionWithNewUserAndNewDevice()
        {
            var device = DeviceHelper.NewDevice();
            var user = UserHelper.NewUser();
            var conn = Connection.New("my_device")
                .FromNewArticle("device", device)
                .ToNewArticle("user", user);
            await conn.SaveAsync();

            Assert.IsTrue(string.IsNullOrWhiteSpace(conn.Id) == false);
            Console.WriteLine("Created connection with id: {0}", conn.Id);
        }

        [TestMethod]
        public async Task GetConnectedArticlesOnConnectionWithSameTypeAndDifferentLabels()
        {
            var parent = await ObjectHelper.CreateNewAsync();
            var child1 = await ObjectHelper.CreateNewAsync();
            var child2 = await ObjectHelper.CreateNewAsync();
            // Create connections
            await Connection.New("link")
                .FromExistingArticle("parent", parent.Id)
                .ToExistingArticle("child", child1.Id)
                .SaveAsync();

            await Connection.New("link")
                .FromExistingArticle("parent", parent.Id)
                .ToExistingArticle("child", child2.Id)
                .SaveAsync();

            // Get connected articles
            var articles = await parent.GetConnectedArticlesAsync("link", label: "child");
            Assert.IsTrue(articles.Count == 2);
            Assert.IsTrue(articles.Select(a => a.Id).Intersect(new[] { child1.Id, child2.Id }).Count() == 2);
        }

        [TestMethod]
        public async Task GetConnectedArticlesForSameTypeAndDifferentLabelsWithoutLabelTest()
        {
            var parent = await ObjectHelper.CreateNewAsync();
            var child1 = await ObjectHelper.CreateNewAsync();
            var child2 = await ObjectHelper.CreateNewAsync();
            // Create connections
            await Connection.New("link")
                .FromExistingArticle("parent", parent.Id)
                .ToExistingArticle("child", child1.Id)
                .SaveAsync();

            await Connection.New("link")
                .FromExistingArticle("parent", parent.Id)
                .ToExistingArticle("child", child2.Id)
                .SaveAsync();

            // Get connected articles
            try
            {
                var articles = await parent.GetConnectedArticlesAsync("link");
                Assert.Fail("This call should have failed since we did not specify the label to retreive.");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message == "Label is required for edge 'link'.");
            }
        }

        [TestMethod]
        public async Task CreateConnectionBetweenNewArticlesAsyncTest()
        {
            var obj1 = new Article("object");
            var obj2 = new Article("object");
            var conn = Connection
                            .New("sibling")
                            .FromNewArticle("object", obj1)
                            .ToNewArticle("object", obj2);

            await conn.SaveAsync();
            Assert.IsTrue(string.IsNullOrWhiteSpace(conn.Id) == false);
            Console.WriteLine("Created connection with id: {0}", conn.Id);
            Assert.IsTrue(string.IsNullOrWhiteSpace(obj1.Id) == false);
            Console.WriteLine("Created new article with id: {0}", obj1.Id);
            Assert.IsTrue(string.IsNullOrWhiteSpace(obj2.Id) == false);
            Console.WriteLine("Created new article with id: {0}", obj2.Id);
        }


        [TestMethod]
        public async Task UpdateConnectionAsyncTest()
        {
            dynamic conn = Connection
                .New("sibling")
                .FromNewArticle("object", ObjectHelper.NewInstance())
                .ToNewArticle("object", ObjectHelper.NewInstance());
            conn.field1 = "test";
            conn.field2 = 15L;
            await conn.SaveAsync();

            Console.WriteLine("Created connection with id: {0}", conn.Id);

            // Update the connection
            conn.field1 = "updated";
            conn.field2 = 11L;
            await conn.SaveAsync();

            // Get the connection
            Connection read = await Connections.GetAsync("sibling", conn.Id);
            // Asserts
            Assert.IsTrue( read.Get<string>("field1") == "updated");
            Assert.IsTrue( read.Get<int>("field2") == 11L);
        }

        [TestMethod]
        public async Task CreateConnectionBetweenNewAndExistingArticlesAsyncTest()
        {
            var obj1 = await ObjectHelper.CreateNewAsync();
            var obj2 = new Article("object");
            var conn = Connection
                            .New("sibling")
                            .FromExistingArticle("object", obj1.Id)
                            .ToNewArticle("object", obj2);
            await conn.SaveAsync();
            Assert.IsTrue(string.IsNullOrWhiteSpace(conn.Id) == false);
            Console.WriteLine("Created connection with id: {0}", conn.Id);
            Assert.IsTrue(string.IsNullOrWhiteSpace(obj2.Id) == false);
            Console.WriteLine("Created new article with id: {0}", obj1.Id);
            // Ensure that the endpoint ids match
            Assert.IsTrue(conn.Endpoints.ToArray().Select(x => x.ArticleId).Intersect(new[] { obj1.Id, obj2.Id }).Count() == 2);
        }


        [TestMethod]
        public async Task UpdatePartialConnectionAsyncTest()
        {
            // Create new connection
            var obj1 = await ObjectHelper.CreateNewAsync();
            var obj2 = new Article("object");
            var conn = Connection
                            .New("sibling")
                            .FromExistingArticle("object", obj1.Id)
                            .ToNewArticle("object", obj2);
            await conn.SaveAsync();

            // Update
            var conn2 = new Connection("sibling", conn.Id);
            var value = Guid.NewGuid().ToString();
            conn["field1"] = value;
            await conn.SaveAsync();

            var updated = await Connections.GetAsync("sibling", conn.Id);
            Assert.IsTrue(updated.Get<string>("field1") == value);
        }

        [TestMethod]
        public async Task CreateConnection2BetweenNewAndExistingArticlesAsyncTest()
        {
            var obj1 = await ObjectHelper.CreateNewAsync();
            var obj2 = new Article("object");
            var conn = Connection
                            .New("sibling")
                            .FromExistingArticle("object", obj1.Id)
                            .ToNewArticle("object", obj2);
            await conn.SaveAsync();
            Assert.IsTrue(string.IsNullOrWhiteSpace(conn.Id) == false);
            Console.WriteLine("Created connection with id: {0}", conn.Id);
            Assert.IsTrue(string.IsNullOrWhiteSpace(obj2.Id) == false);
            Console.WriteLine("Created new article with id: {0}", obj1.Id);
            // Ensure that the endpoint ids match
            Assert.IsTrue(conn.Endpoints.ToArray().Select(x => x.ArticleId).Intersect(new[] { obj1.Id, obj2.Id }).Count() == 2);
        }

        [TestMethod]
        public async Task CreateConnectionBetweenExistingArticlesAsyncTest()
        {
            var obj1 = await ObjectHelper.CreateNewAsync();
            var obj2 = await ObjectHelper.CreateNewAsync();
            var conn = Connection
                            .New("sibling")
                            .FromExistingArticle("object", obj1.Id)
                            .ToExistingArticle("object", obj2.Id);
            await conn.SaveAsync();
            Assert.IsTrue(string.IsNullOrWhiteSpace(conn.Id) == false);
            Console.WriteLine("Created connection with id: {0}", conn.Id);
            Assert.IsTrue(string.IsNullOrWhiteSpace(obj2.Id) == false);
            Console.WriteLine("Created new article with id: {0}", obj1.Id);
            // Ensure that the endpoint ids match
            Assert.IsTrue(conn.Endpoints.ToArray().Select(x => x.ArticleId).Intersect(new[] { obj1.Id, obj2.Id }).Count() == 2);
        }

        [TestMethod]
        public async Task GetConnectionAsyncTest()
        {
            // Create a new connection
            var conn = await ConnectionHelper.CreateNew();
            var read = await Connections.GetAsync(conn.Type, conn.Id);
            Assert.IsTrue(read != null);
            Assert.IsTrue(read.Id == conn.Id);
            Assert.IsTrue(conn.Endpoints.ToArray().Select(x => x.ArticleId).Intersect(read.Endpoints.ToArray().Select( x => x.ArticleId)).Count() == 2);
        }

        [TestMethod]
        public async Task GetConnectionByEndpointsAsyncTest()
        {
            // Create a new connection
            var conn = await ConnectionHelper.CreateNew();
            var endpoints = conn.Endpoints.ToArray();
            var read = await Connections.GetAsync(conn.Type, endpoints[0].ArticleId, endpoints[1].ArticleId);
            Assert.IsTrue(read != null);
            Assert.IsTrue(read.Id == conn.Id);
            Assert.IsTrue(conn.Endpoints.ToArray().Select(x => x.ArticleId).Intersect(read.Endpoints.ToArray().Select(x => x.ArticleId)).Count() == 2);
        }

        [TestMethod]
        public async Task FindAllConnectionsAsyncTest()
        {
            // Create a new connection
            var conn = await ConnectionHelper.CreateNew();
            // Find all
            var connections = await Connections.FindAllAsync("sibling");
            Assert.IsTrue(connections != null);
            Assert.IsTrue(connections.Count > 0);
            Console.WriteLine("Total connections: {0}", connections.TotalRecords);
        }

        [TestMethod]
        public async Task DeleteConnectionAsyncTest()
        {

            // Create a new connection
            var conn = await ConnectionHelper.CreateNew();
            // Delete the connection
            await Connections.DeleteAsync(conn.Type, conn.Id);
            // Try and get the connection
            try
            {
                var read = await Connections.GetAsync(conn.Type, conn.Id);
                Assert.Fail("No exception was raised on reading deleted connection.");
            }
            catch (Net45.AppacitiveException aex)
            {
                Assert.IsTrue(aex.Code == "404");
            }
        }

        [TestMethod]
        public async Task BulkDeleteConnectionTest()
        {
            // Create a new connection
            var conn1 = await ConnectionHelper.CreateNew();
            var conn2 = await ConnectionHelper.CreateNew();
            // Delete the connection
            await Connections.MultiDeleteAsync(conn1.Type, new[] { conn1.Id, conn2.Id });
            // Try and get the connection
            try
            {
                var read = await Connections.GetAsync(conn1.Type, conn1.Id);
                Assert.Fail("No exception was raised on reading deleted connection.");
            }
            catch (Net45.AppacitiveException aex)
            {
                Assert.IsTrue(aex.Code == "404");
            }

            try
            {
                var read = await Connections.GetAsync(conn2.Type, conn2.Id);
                Assert.Fail("No exception was raised on reading deleted connection.");
            }
            catch (Net45.AppacitiveException aex)
            {
                Assert.IsTrue(aex.Code == "404");
            }
        }
    }
}
