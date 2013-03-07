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
            Assert.IsTrue(conn.EndpointA.ArticleId == obj1.Id || conn.EndpointB.ArticleId == obj1.Id);
            Assert.IsTrue(conn.EndpointA.ArticleId == obj2.Id || conn.EndpointB.ArticleId == obj2.Id);
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
            Assert.IsTrue(conn.EndpointA.ArticleId == obj1.Id || conn.EndpointB.ArticleId == obj1.Id);
            Assert.IsTrue(conn.EndpointA.ArticleId == obj2.Id || conn.EndpointB.ArticleId == obj2.Id);
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
            Assert.IsTrue(conn.EndpointA.ArticleId == obj1.Id || conn.EndpointB.ArticleId == obj1.Id);
            Assert.IsTrue(conn.EndpointA.ArticleId == obj2.Id || conn.EndpointB.ArticleId == obj2.Id);
        }

        [TestMethod]
        public async Task GetConnectionAsyncTest()
        {
            // Create a new connection
            var conn = await ConnectionHelper.CreateNew();
            var read = await Connection.GetAsync(conn.Type, conn.Id);
            Assert.IsTrue(read != null);
            Assert.IsTrue(read.Id == conn.Id);
            Assert.IsTrue(read.EndpointA.ArticleId == conn.EndpointA.ArticleId || read.EndpointA.ArticleId == conn.EndpointB.ArticleId);
            Assert.IsTrue(read.EndpointB.ArticleId == conn.EndpointA.ArticleId || read.EndpointB.ArticleId == conn.EndpointB.ArticleId);
        }

        [TestMethod]
        public async Task GetConnectionByEndpointsAsyncTest()
        {
            // Create a new connection
            var conn = await ConnectionHelper.CreateNew();
            var read = await Connection.GetAsync(conn.Type, conn.EndpointA.ArticleId, conn.EndpointB.ArticleId);
            Assert.IsTrue(read != null);
            Assert.IsTrue(read.Id == conn.Id);
            Assert.IsTrue(read.EndpointA.ArticleId == conn.EndpointA.ArticleId || read.EndpointA.ArticleId == conn.EndpointB.ArticleId);
            Assert.IsTrue(read.EndpointB.ArticleId == conn.EndpointA.ArticleId || read.EndpointB.ArticleId == conn.EndpointB.ArticleId);
        }

        [TestMethod]
        public async Task FindAllConnectionsAsyncTest()
        {
            // Create a new connection
            var conn = await ConnectionHelper.CreateNew();
            // Find all
            var connections = await Connection.FindAllAsync("sibling");
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
            await Connection.DeleteAsync(conn.Type, conn.Id);
            // Try and get the connection
            try
            {
                var read = await Connection.GetAsync(conn.Type, conn.Id);
                Assert.Fail("No exception was raised on reading deleted connection.");
            }
            catch (AppacitiveException aex)
            {
                Assert.IsTrue(aex.Code == "404");
            }
        }
    }
}
