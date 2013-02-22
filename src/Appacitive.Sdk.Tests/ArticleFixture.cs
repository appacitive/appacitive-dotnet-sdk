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
    public class ArticleFixture
    {
        [TestMethod]
        public void CreateArticleTest()
        {
            dynamic obj = new Article("object");
            obj.intfield = 1;
            obj.decimalfield = 22m / 7m;
            obj.Save();
            var saved = obj as Article;
            Assert.IsNotNull(saved);
            Assert.IsTrue(string.IsNullOrWhiteSpace(saved.Id) == false);
            Console.WriteLine("Created article with id {0}.", saved.Id);
        }

        [TestMethod]
        public void CreateArticleAsyncTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
                {
                    try
                    {
                        dynamic obj = new Article("object");
                        obj.intfield = 1;
                        obj.decimalfield = 22m / 7m;
                        await obj.SaveAsync();
                        var saved = obj as Article;
                        Assert.IsNotNull(saved);
                        Assert.IsTrue(string.IsNullOrWhiteSpace(saved.Id) == false);
                        Console.WriteLine("Created article with id {0}.", saved.Id);
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
        public void GetArticleTest()
        {
            dynamic obj = new Article("object");
            obj.intfield = 1;
            var pi = 22m / 7;
            obj.decimalfield = pi;
            obj.Save();
            var saved = obj as Article;
            Assert.IsNotNull(saved);
            Assert.IsTrue(string.IsNullOrWhiteSpace(saved.Id) == false);
            Console.WriteLine("Created article with id {0}.", saved.Id);

            dynamic copy = Article.Get("object", saved.Id);
            Assert.IsNotNull(copy);
            int intfield = copy.intfield;
            decimal decimalField = copy.decimalfield;
            Assert.IsTrue(intfield == 1);
            Assert.IsTrue(decimalField == pi);
        }

        [TestMethod]
        public void GetArticleAsyncTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
                {
                    try
                    {
                        dynamic obj = new Article("object");
                        obj.intfield = 1;
                        var pi = 22m / 7;
                        obj.decimalfield = pi;
                        await obj.SaveAsync();
                        var saved = obj as Article;
                        Assert.IsNotNull(saved);
                        Assert.IsTrue(string.IsNullOrWhiteSpace(saved.Id) == false);
                        Console.WriteLine("Created article with id {0}.", saved.Id);

                        dynamic copy = await Article.GetAsync("object", saved.Id);
                        Assert.IsNotNull(copy);
                        int intfield = copy.intfield;
                        decimal decimalField = copy.decimalfield;
                        Assert.IsTrue(intfield == 1);
                        Assert.IsTrue(decimalField == pi);
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
        public void DeleteArticleTest()
        {
            // Create the article
            dynamic obj = new Article("object");
            obj.intfield = 1;
            var pi = 22m / 7;
            obj.decimalfield = pi;
            obj.Save();
            var saved = obj as Article;
            Assert.IsNotNull(saved);
            Assert.IsTrue(string.IsNullOrWhiteSpace(saved.Id) == false);
            Console.WriteLine("Created article with id {0}.", saved.Id);

            // Delete the article
            Article.Delete("object", saved.Id);

            // Try and get and confirm that the article is deleted.
            try
            {
                var copy = Article.Get("object", saved.Id);
                Assert.Fail("Operation should have faulted since the article has been deleted.");
            }
            catch (AppacitiveException ex)
            {
                var msg = string.Format("Cannot locate article of type 'object' and id {0}.", saved.Id);
                Assert.IsTrue(ex.Message == msg);
            }
        }

        [TestMethod]
        public void DeleteArticleAsyncTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
                        {
                            try
                            {
                                // Create the article
                                dynamic obj = new Article("object");
                                obj.intfield = 1;
                                var pi = 22m / 7;
                                obj.decimalfield = pi;
                                await obj.SaveAsync();
                                var saved = obj as Article;
                                Assert.IsNotNull(saved);
                                Assert.IsTrue(string.IsNullOrWhiteSpace(saved.Id) == false);
                                Console.WriteLine("Created article with id {0}.", saved.Id);

                                // Delete the article
                                await Article.DeleteAsync("object", saved.Id);

                                // Try and get and confirm that the article is deleted.
                                try
                                {
                                    var copy = await Article.GetAsync("object", saved.Id);
                                    Assert.Fail("Operation should have faulted since the article has been deleted.");
                                }
                                catch (AppacitiveException ex)
                                {
                                    var msg = string.Format("Cannot locate article of type 'object' and id {0}.", saved.Id);
                                    Assert.IsTrue(ex.Message == msg);
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

        [TestMethod]
        public void UpdateArticleTest()
        {
            // Create the article
            dynamic obj = new Article("object");
            obj.intfield = 1;
            var pi = 22m / 7;
            obj.decimalfield = pi;
            obj.Save();
            var saved = obj as Article;
            Assert.IsNotNull(saved);
            Assert.IsTrue(string.IsNullOrWhiteSpace(saved.Id) == false);
            Console.WriteLine("Created article with id {0}.", saved.Id);

            // Get the newly created article
            dynamic copy = Article.Get("object", saved.Id);
            Assert.IsNotNull(copy);
            int intfield = copy.intfield;
            decimal decimalField = copy.decimalfield;
            Assert.IsTrue(intfield == 1);
            Assert.IsTrue(decimalField == pi);

            // Update the article
            copy.intfield = 2;
            copy.decimalfield = 30m;
            copy.stringfield = "Test";
            copy.Save();

            // Get updated copy and verify
            dynamic updated = Article.Get("object", saved.Id);
            Assert.IsNotNull(updated);
            intfield = updated.intfield;
            decimalField = updated.decimalfield;
            string stringField = updated.stringfield;

            Assert.IsTrue(intfield == 2, "intfield not updated.");
            Assert.IsTrue(decimalField == 30, "decimal field not updated.");
            Assert.IsTrue(stringField == "Test", "stringfield not updated.");
        }

        [TestMethod]
        public void UpdateArticleAsyncTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
            {
                try
                {
                    // Create the article
                    dynamic obj = new Article("object");
                    obj.intfield = 1;
                    var pi = 22m / 7;
                    obj.decimalfield = pi;
                    await obj.SaveAsync();
                    var saved = obj as Article;
                    Assert.IsNotNull(saved);
                    Assert.IsTrue(string.IsNullOrWhiteSpace(saved.Id) == false);
                    Console.WriteLine("Created article with id {0}.", saved.Id);

                    // Get the newly created article
                    dynamic copy = await Article.GetAsync("object", saved.Id);
                    Assert.IsNotNull(copy);
                    int intfield = copy.intfield;
                    decimal decimalField = copy.decimalfield;
                    Assert.IsTrue(intfield == 1);
                    Assert.IsTrue(decimalField == pi);

                    // Update the article
                    copy.intfield = 2;
                    copy.decimalfield = 30m;
                    copy.stringfield = "Test";
                    await copy.SaveAsync();

                    // Get updated copy and verify
                    dynamic updated = await Article.GetAsync("object", saved.Id);
                    Assert.IsNotNull(updated);
                    intfield = updated.intfield;
                    decimalField = updated.decimalfield;
                    string stringField = updated.stringfield;

                    Assert.IsTrue(intfield == 2, "intfield not updated.");
                    Assert.IsTrue(decimalField == 30, "decimal field not updated.");
                    Assert.IsTrue(stringField == "Test", "stringfield not updated.");
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
        public void FindAllArticlesTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
                {
                    try
                    {
                        // Create the article
                        dynamic obj = new Article("object");
                        obj.stringfield = Unique.String;
                        await obj.SaveAsync();
                        var saved = obj as Article;
                        Console.WriteLine("Created articled with id {0}", saved.Id);

                        // Search
                        var articles = await Article.FindAllAsync("object");
                        articles.ForEach(a => Console.WriteLine(a.Id));
                        Console.WriteLine("page:{0} pageSize:{1} total: {2}", articles.PageNumber, articles.PageSize, articles.TotalRecords);
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
        public void FindAllArticlesWithQueryTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
            {
                try
                {
                    // Create the article
                    dynamic obj = new Article("object");
                    obj.stringfield = Unique.String;
                    await obj.SaveAsync();
                    var saved = obj as Article;
                    Console.WriteLine("Created articled with id {0}", saved.Id);

                    // Search
                    string stringToSearch = obj.stringfield;
                    var articles = await Article.FindAllAsync("object", Query.Property("stringfield").IsEqualTo(stringToSearch));
                    Assert.IsNotNull(articles);
                    Assert.IsTrue(articles.Count == 1);
                    Console.WriteLine("page:{0} pageSize:{1} total: {2}", articles.PageNumber, articles.PageSize, articles.TotalRecords);
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
        public void FindAllArticlesWithNestedQueryTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
            {
                try
                {
                    // Create the article
                    dynamic obj = new Article("object");
                    obj.stringfield = Unique.String;
                    obj.intfield = 10;
                    await obj.SaveAsync();
                    var saved = obj as Article;
                    Console.WriteLine("Created articled with id {0}", saved.Id);

                    // Search
                    string stringToSearch = obj.stringfield;
                    var query = BooleanOperator.And(new[] 
                        {
                            Query.Property("stringfield").IsEqualTo(stringToSearch),
                            Query.Property("intfield").IsEqualTo(10)
                        });

                    var articles = await Article.FindAllAsync("object", query);
                    Assert.IsNotNull(articles);
                    Assert.IsTrue(articles.Count == 1);
                    Console.WriteLine("page:{0} pageSize:{1} total: {2}", articles.PageNumber, articles.PageSize, articles.TotalRecords);
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
        public void FindNonExistantPageTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
            {
                try
                {
                    // Search
                    var articles = await Article.FindAllAsync("object", 10000, 500);
                    Assert.IsNotNull(articles);
                    Console.WriteLine("page:{0} pageSize:{1} total: {2}", articles.PageNumber, articles.PageSize, articles.TotalRecords);
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
        public void FindAndDisplayAllArticlesTest()
        {
            var waitHandle = new ManualResetEvent(false);
            Exception fault = null;
            Action action = async () =>
            {
                try
                {
                    // Create the article
                    dynamic obj = new Article("object");
                    obj.stringfield = Unique.String;
                    await obj.SaveAsync();
                    var saved = obj as Article;
                    Console.WriteLine("Created articled with id {0}", saved.Id);
                    var index = 1;
                    // Search
                    var articles = await Article.FindAllAsync("object", 1, 100);
                    do
                    {   
                        articles.ForEach(a => Console.WriteLine("{0}) {1}", index++, a.Id));
                        Console.WriteLine("page:{0} pageSize:{1} total: {2}", articles.PageNumber, articles.PageSize, articles.TotalRecords);
                        if (articles.IsLastPage == false)
                            articles = await articles.NextPageAsync();
                        else
                            break;
                    } while (true);
                    Console.WriteLine("Finished.");
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

    public static class Async
    {
        public static void Run(Action run)
        {
            var waitHandle = new ManualResetEvent(false);
            Action action = () =>
            {
                try
                {
                    run();
                }
                finally
                {
                    waitHandle.Set();
                }
            };
            action();
            waitHandle.WaitOne();
        }
    }
}
