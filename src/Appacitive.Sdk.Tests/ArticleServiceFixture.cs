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
    public class ArticleServiceFixture
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
        public void CreateArticleTest()
        {
            // Create article
            var now = DateTime.Now;
            dynamic obj = new Article("object");
            obj.intfield = 1;
            obj.decimalfield = 10.0m;
            obj.datefield = "2012-12-20";
            obj.datetimefield = now.ToString("o");
            obj.stringfield = "string value";
            obj.textfield = "text value";
            obj.boolfield = false;
            obj.geofield = "11.5,12.5";
            obj.listfield = "a";
            obj.SetAttribute("attr1","value1");
            obj.SetAttribute("attr2", "value2");

            var service = ObjectFactory.Build<IArticleService>();
            CreateArticleResponse response = null;
            var timeTaken = Measure.TimeFor(() =>
                {
                    response = service.CreateArticle(new CreateArticleRequest()
                    {
                        Article = obj,
                        SessionToken = AppacitiveContext.SessionToken,
                        UserToken = AppacitiveContext.UserToken,
                        Environment = Env
                    });
                });
            Console.WriteLine("Time taken for call: {0} seconds", timeTaken);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Status);
            Assert.IsTrue(response.Status.IsSuccessful);
            Assert.IsNotNull(response.Article);
            Console.WriteLine("Created article id {0}", response.Article.Id);
            Console.WriteLine("Time taken: {0} seconds", response.TimeTaken);
        }

        [TestMethod]
        public void CreateArticleAsyncTest()
        {
            // Initialize the app
            App.Initialize(ApiKey, Env);
            // Create article
            var now = DateTime.Now;
            dynamic obj = new Article("object");
            obj.intfield = 1;
            obj.decimalfield = 10.0m;
            obj.datefield = "2012-12-20";
            obj.datetimefield = now.ToString("o");
            obj.stringfield = "string value";
            obj.textfield = "text value";
            obj.boolfield = false;
            obj.geofield = "11.5,12.5";
            obj.listfield = "a";

            var service = ObjectFactory.Build<IArticleService>();
            CreateArticleResponse response = null;
            var waitHandle = new ManualResetEvent(false);
            Action action = async () =>
            {
                response = await service.CreateArticleAsync(new CreateArticleRequest()
                {
                    Article = obj,
                    SessionToken = AppacitiveContext.SessionToken,
                    UserToken = AppacitiveContext.UserToken,
                    Environment = Env
                });
                waitHandle.Set();
            };
            action();
            waitHandle.WaitOne();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Status);
            Assert.IsTrue(response.Status.IsSuccessful);
            Assert.IsNotNull(response.Article);
            Console.WriteLine("Created article id {0}", response.Article.Id);
            Console.WriteLine("Time taken {0} seconds", response.TimeTaken);
        }

        [TestMethod]
        public void GetArticleTest()
        {
            // Create article
            var now = DateTime.Now;
            dynamic obj = new Article("object");
            obj.intfield = 1;
            obj.decimalfield = 10.0m;
            obj.datefield = "2012-12-20";
            obj.datetimefield = now.ToString("o");
            obj.stringfield = "string value";
            obj.textfield = "text value";
            obj.boolfield = false;
            obj.geofield = "11.5,12.5";
            obj.listfield = "a";

            var service = ObjectFactory.Build<IArticleService>();
            CreateArticleResponse response = null;
            response = service.CreateArticle(new CreateArticleRequest()
            {
                Article = obj,
                SessionToken = AppacitiveContext.SessionToken,
                UserToken = AppacitiveContext.UserToken,
                Environment = Env
            });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Status);
            Assert.IsTrue(response.Status.IsSuccessful);
            Assert.IsNotNull(response.Article);
            Console.WriteLine("Created article id {0}", response.Article.Id);
            Console.WriteLine("Time taken: {0} seconds", response.TimeTaken);


            // Get the article
            GetArticleResponse getResponse = null;
            getResponse = service.GetArticle(
                new GetArticleRequest()
                {
                    SessionToken = AppacitiveContext.SessionToken,
                    Id = response.Article.Id,
                    Type = response.Article.Type,
                    Environment = AppacitiveContext.Environment
                });
            Assert.IsNotNull(getResponse);
            Assert.IsNotNull(getResponse.Status);
            Assert.IsTrue(getResponse.Status.IsSuccessful);
            Assert.IsNotNull(getResponse.Article);
            Console.WriteLine("Successfully read article id {0}", getResponse.Article.Id);
            Console.WriteLine("Time taken: {0} seconds", getResponse.TimeTaken);

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
                        // Create article
                        var now = DateTime.Now;
                        dynamic obj = new Article("object");
                        obj.intfield = 1;
                        obj.decimalfield = 10.0m;
                        obj.datefield = "2012-12-20";
                        obj.datetimefield = now.ToString("o");
                        obj.stringfield = "string value";
                        obj.textfield = "text value";
                        obj.boolfield = false;
                        obj.geofield = "11.5,12.5";
                        obj.listfield = "a";

                        var service = ObjectFactory.Build<IArticleService>();
                        CreateArticleResponse response = null;
                        response = await service.CreateArticleAsync(new CreateArticleRequest()
                        {
                            Article = obj,
                            SessionToken = AppacitiveContext.SessionToken,
                            UserToken = AppacitiveContext.UserToken,
                            Environment = Env
                        });

                        Assert.IsNotNull(response);
                        Assert.IsNotNull(response.Status);
                        Assert.IsTrue(response.Status.IsSuccessful);
                        Assert.IsNotNull(response.Article);
                        Console.WriteLine("Created article id {0}", response.Article.Id);
                        Console.WriteLine("Time taken: {0} seconds", response.TimeTaken);


                        // Get the article
                        GetArticleResponse getResponse = null;
                        getResponse = await service.GetArticleAsync(
                            new GetArticleRequest()
                            {
                                SessionToken = AppacitiveContext.SessionToken,
                                Id = response.Article.Id,
                                Type = response.Article.Type,
                                Environment = AppacitiveContext.Environment
                            });
                        Assert.IsNotNull(getResponse);
                        Assert.IsNotNull(getResponse.Status);
                        Assert.IsTrue(getResponse.Status.IsSuccessful);
                        Assert.IsNotNull(getResponse.Article);
                        Console.WriteLine("Successfully read article id {0}", getResponse.Article.Id);
                        Console.WriteLine("Time taken: {0} seconds", getResponse.TimeTaken);
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
            // Run async
            action();
            waitHandle.WaitOne();
            if (fault != null)
                throw fault;
        }

        [TestMethod]
        public void DeleteArticleTest()
        {
            // Create article
            var now = DateTime.Now;
            dynamic obj = new Article("object");
            obj.intfield = 1;
            obj.decimalfield = 10.0m;
            obj.datefield = "2012-12-20";
            obj.datetimefield = now.ToString("o");
            obj.stringfield = "string value";
            obj.textfield = "text value";
            obj.boolfield = false;
            obj.geofield = "11.5,12.5";
            obj.listfield = "a";
            obj.SetAttribute("attr1", "value1");
            obj.SetAttribute("attr2", "value2");

            var service = ObjectFactory.Build<IArticleService>();
            CreateArticleResponse response = null;
            var timeTaken = Measure.TimeFor(() =>
            {
                response = service.CreateArticle(new CreateArticleRequest()
                {
                    Article = obj,
                    SessionToken = AppacitiveContext.SessionToken,
                    UserToken = AppacitiveContext.UserToken,
                    Environment = Env
                });
            });
            Console.WriteLine("Time taken for call: {0} seconds", timeTaken);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Status);
            Assert.IsTrue(response.Status.IsSuccessful);
            Assert.IsNotNull(response.Article);
            Console.WriteLine("Created article id {0}", response.Article.Id);
            Console.WriteLine("Time taken: {0} seconds", response.TimeTaken);    

            // Delete the article
            Status deleteArticleResponse = null;
            deleteArticleResponse = service.DeleteArticle(new DeleteArticleRequest()
            {
                Id = response.Article.Id,
                Type = response.Article.Type,
                Environment = AppacitiveContext.Environment,
                SessionToken = AppacitiveContext.SessionToken
            });
            Assert.IsNotNull(deleteArticleResponse, "Delete articler response is null.");
            Assert.IsTrue(deleteArticleResponse.IsSuccessful == true, deleteArticleResponse.Message ?? "Delete article operation failed.");

            // Try get the deleted article
            var getArticleResponse = service.GetArticle(
                new GetArticleRequest()
                {
                    Id = response.Article.Id,
                    Type = response.Article.Type,
                    SessionToken = AppacitiveContext.SessionToken,
                    Environment = AppacitiveContext.Environment
                });
            Assert.IsNotNull(getArticleResponse, "Get article response is null.");
            Assert.IsNull(getArticleResponse.Article, "Should not be able to get a deleted article.");
            Assert.IsTrue(getArticleResponse.Status.Code == "404", "Error code expected was not 404.");
        }

        [TestMethod]
        public void DeleteArticleAsyncTest()
        {
            Exception fault = null;
            var waitHandle = new ManualResetEvent(false);
            var action = new Action(async () =>
                {
                    try
                    {
                        // Create article
                        var now = DateTime.Now;
                        dynamic obj = new Article("object");
                        obj.intfield = 1;
                        obj.decimalfield = 10.0m;
                        obj.datefield = "2012-12-20";
                        obj.datetimefield = now.ToString("o");
                        obj.stringfield = "string value";
                        obj.textfield = "text value";
                        obj.boolfield = false;
                        obj.geofield = "11.5,12.5";
                        obj.listfield = "a";
                        obj.SetAttribute("attr1", "value1");
                        obj.SetAttribute("attr2", "value2");

                        var service = ObjectFactory.Build<IArticleService>();
                        CreateArticleResponse response = null;
                        response = await service.CreateArticleAsync(new CreateArticleRequest()
                        {
                            Article = obj,
                            SessionToken = AppacitiveContext.SessionToken,
                            UserToken = AppacitiveContext.UserToken,
                            Environment = Env
                        });
                        Assert.IsNotNull(response);
                        Assert.IsNotNull(response.Status);
                        Assert.IsTrue(response.Status.IsSuccessful);
                        Assert.IsNotNull(response.Article);
                        Console.WriteLine("Created article id {0}", response.Article.Id);
                        Console.WriteLine("Time taken: {0} seconds", response.TimeTaken);

                        // Delete the article
                        Status deleteArticleResponse = null;
                        deleteArticleResponse = await service.DeleteArticleAsync(new DeleteArticleRequest()
                        {
                            Id = response.Article.Id,
                            Type = response.Article.Type,
                            Environment = AppacitiveContext.Environment,
                            SessionToken = AppacitiveContext.SessionToken
                        });
                        Assert.IsNotNull(deleteArticleResponse, "Delete articler response is null.");
                        Assert.IsTrue(deleteArticleResponse.IsSuccessful == true, deleteArticleResponse.Message ?? "Delete article operation failed.");

                        // Try get the deleted article
                        var getArticleResponse = await service.GetArticleAsync(
                            new GetArticleRequest()
                            {
                                Id = response.Article.Id,
                                Type = response.Article.Type,
                                SessionToken = AppacitiveContext.SessionToken,
                                Environment = AppacitiveContext.Environment
                            });
                        Assert.IsNotNull(getArticleResponse, "Get article response is null.");
                        Assert.IsNull(getArticleResponse.Article, "Should not be able to get a deleted article.");
                        Assert.IsTrue(getArticleResponse.Status.Code == "404", "Error code expected was not 404.");
                    }
                    catch (Exception ex)
                    {
                        fault = ex;
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                });
            action();
            waitHandle.WaitOne();
            Assert.IsNull(fault);
        }



        [TestMethod]
        public void UpdateArticleTest()
        {
            // Create an article
            var now = DateTime.Now;
            dynamic obj = new Article("object");
            obj.intfield = 1;
            obj.decimalfield = 10.0m;
            obj.datefield = "2012-12-20";
            obj.stringfield = "initial";
            obj.Tags.Add("initial");

            var service = ObjectFactory.Build<IArticleService>();
            var createdResponse = service.CreateArticle(new CreateArticleRequest()
            {
                Article = obj,
                Environment = AppacitiveContext.Environment,
                SessionToken = AppacitiveContext.SessionToken
            });
            Assert.IsNotNull(createdResponse, "Article creation failed.");
            Assert.IsNotNull(createdResponse.Status, "Status is null.");
            Assert.IsTrue(createdResponse.Status.IsSuccessful, createdResponse.Status.Message ?? "Create article failed.");
            var created = createdResponse.Article;

            // Update the article
            var updateRequest = new UpdateArticleRequest() { 
                Id = created.Id, 
                Type = created.Type,
                SessionToken = AppacitiveContext.SessionToken,
                Environment = AppacitiveContext.Environment
            };
            updateRequest.PropertyUpdates["intfield"] = "2";
            updateRequest.PropertyUpdates["decimalfield"] = 20.0m.ToString();
            updateRequest.PropertyUpdates["stringfield"] = null;
            updateRequest.PropertyUpdates["datefield"] = "2013-11-20";
            updateRequest.AddedTags.AddRange(new [] {"tag1", "tag2" });
            updateRequest.RemovedTags.AddRange(new[] { "initial" });
            var updatedResponse = service.UpdateArticle(updateRequest);

            Assert.IsNotNull(updatedResponse, "Update article response is null.");
            Assert.IsNotNull(updatedResponse.Status, "Update article response status is null.");
            Assert.IsNotNull(updatedResponse.Article, "Updated article is null.");
            var updated = updatedResponse.Article;
            Assert.IsTrue(updated["intfield"] == "2");
            Assert.IsTrue(updated["decimalfield"] == "20.0");
            Assert.IsTrue(updated["stringfield"] == null);
            Assert.IsTrue(updated["datefield"] == "2013-11-20");
            Assert.IsTrue(updated.Tags.Count() == 2);
            Assert.IsTrue(updated.Tags.Intersect(new[] { "tag1", "tag2" }).Count() == 2);
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
                        // Create an article
                        var now = DateTime.Now;
                        dynamic obj = new Article("object");
                        obj.intfield = 1;
                        obj.decimalfield = 10.0m;
                        obj.datefield = "2012-12-20";
                        obj.stringfield = "initial";
                        obj.Tags.Add("initial");

                        var service = ObjectFactory.Build<IArticleService>();
                        var createdResponse = await service.CreateArticleAsync(new CreateArticleRequest()
                        {
                            Article = obj,
                            Environment = AppacitiveContext.Environment,
                            SessionToken = AppacitiveContext.SessionToken
                        });
                        Assert.IsNotNull(createdResponse, "Article creation failed.");
                        Assert.IsNotNull(createdResponse.Status, "Status is null.");
                        Assert.IsTrue(createdResponse.Status.IsSuccessful, createdResponse.Status.Message ?? "Create article failed.");
                        var created = createdResponse.Article;

                        // Update the article
                        var updateRequest = new UpdateArticleRequest()
                        {
                            Id = created.Id,
                            Type = created.Type,
                            SessionToken = AppacitiveContext.SessionToken,
                            Environment = AppacitiveContext.Environment
                        };
                        updateRequest.PropertyUpdates["intfield"] = "2";
                        updateRequest.PropertyUpdates["decimalfield"] = 20.0m.ToString();
                        updateRequest.PropertyUpdates["stringfield"] = null;
                        updateRequest.PropertyUpdates["datefield"] = "2013-11-20";
                        updateRequest.AddedTags.AddRange(new[] { "tag1", "tag2" });
                        updateRequest.RemovedTags.AddRange(new[] { "initial" });
                        var updatedResponse = await service.UpdateArticleAsync(updateRequest);

                        Assert.IsNotNull(updatedResponse, "Update article response is null.");
                        Assert.IsNotNull(updatedResponse.Status, "Update article response status is null.");
                        Assert.IsNotNull(updatedResponse.Article, "Updated article is null.");
                        var updated = updatedResponse.Article;
                        Assert.IsTrue(updated["intfield"] == "2");
                        Assert.IsTrue(updated["decimalfield"] == "20.0");
                        Assert.IsTrue(updated["stringfield"] == null);
                        Assert.IsTrue(updated["datefield"] == "2013-11-20");
                        Assert.IsTrue(updated.Tags.Count() == 2);
                        Assert.IsTrue(updated.Tags.Intersect(new[] { "tag1", "tag2" }).Count() == 2);
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
            Assert.IsNull(fault);

        }
        
    }
}
