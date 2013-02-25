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
        public void CreateConnectionBetweenNewArticles()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
                {
                    try
                    {
                        var obj1 = new Article("object");
                        var obj2 = new Article("object");
                        var conn = Connection
                                        .Create("sibling")
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
        public void CreateConnectionBetweenNewAndExistingArticles()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
            {
                try
                {
                    var obj1 = ObjectHelper.CreateNew();
                    var obj2 = new Article("object");
                    var conn = Connection
                                    .Create("sibling")
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
        public void CreateConnectionBetweenExistingArticles()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
            {
                try
                {   
                    var obj1 = ObjectHelper.CreateNew();
                    var obj2 = ObjectHelper.CreateNew();
                    var conn = Connection
                                    .Create("sibling")
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
        public void GetConnectionTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
            {
                try
                {
                    // Create a new connection
                    var conn = await ConnectionHelper.CreateNew();
                    var read = await Connection.Get(conn.Type, conn.Id);
                    Assert.IsTrue(read != null);
                    Assert.IsTrue(read.Id == conn.Id);
                    Assert.IsTrue(read.EndpointA.ArticleId == conn.EndpointA.ArticleId || read.EndpointA.ArticleId == conn.EndpointB.ArticleId);
                    Assert.IsTrue(read.EndpointB.ArticleId == conn.EndpointA.ArticleId || read.EndpointB.ArticleId == conn.EndpointB.ArticleId);
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
        public void DeleteConnectionTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
            {
                try
                {
                    // Create a new connection
                    var conn = await ConnectionHelper.CreateNew();
                    // Delete the connection
                    await Connection.DeleteAsync(conn.Type, conn.Id);
                    // Try and get the connection
                    try
                    {
                        var read = await Connection.Get(conn.Type, conn.Id);
                        Assert.Fail("No exception was raised on reading deleted connection.");
                    }
                    catch (AppacitiveException aex)
                    {
                        Assert.IsTrue(aex.Code == "404");
                    }
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
