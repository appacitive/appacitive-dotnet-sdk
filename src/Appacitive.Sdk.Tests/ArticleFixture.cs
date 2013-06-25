using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class ArticleFixture
    {
        [TestMethod]
        public async Task CreateArticleAsyncTest()
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

        [TestMethod]
        public async Task GetDevicesShouldReturnDeviceObjectsTest()
        {
            var created = await DeviceHelper.CreateNewAsync();
            var devices = await Articles.FindAllAsync("device");
            Assert.IsFalse(devices.Exists(d => d is Device == false));
        }

        [TestMethod]
        public async Task GetUsersShouldReturnUserObjectsTest()
        {
            var created = await UserHelper.CreateNewUserAsync();
            var users = await Articles.FindAllAsync("user");
            Assert.IsFalse(users.Exists(d => d is User == false));
        }

        [TestMethod]
        public async Task GetArticleAsyncTest()
        {
            // Create new article
            dynamic article = new Article("object");
            decimal pi = 22.0m / 7.0m;
            article.intfield = 1;
            article.decimalfield = pi;
            var saved = await ObjectHelper.CreateNewAsync(article as Article);

            // Get the created article
            dynamic copy = await Articles.GetAsync("object", saved.Id);
            Assert.IsNotNull(copy);
            int intfield = copy.intfield;
            decimal decimalField = copy.decimalfield;
            Assert.IsTrue(intfield == 1);
            Assert.IsTrue(decimalField == pi);

        }

        [TestMethod]
        public async Task MultiValueArticleTest()
        {
            var obj = new Article("object");
            obj.SetList<string>("multifield", new[] { "1", "2", "3", "4" });
            await obj.SaveAsync();

            var read = await Articles.GetAsync("object", obj.Id);
            var value = read.GetList<string>("multifield");
            var strList = read.GetList<string>("multifield");
            var intList = read.GetList<int>("multifield");
        }

        [TestMethod]
        public async Task MultiGetArticleAsyncTest()
        {
            // Create new article
            var obj1 = await ObjectHelper.CreateNewAsync();
            var obj2 = await ObjectHelper.CreateNewAsync();

            // Get the created article
            var enumerable = await Articles.MultiGetAsync("object", new[] { obj1.Id, obj2.Id });

            // Asserts
            Assert.IsNotNull(enumerable);
            var list = enumerable.Select(x => x.Id);
            Assert.IsTrue(list.Intersect(new[] { obj1.Id, obj2.Id }).Count() == 2);

        }

        [TestMethod]
        public async Task BulkDeleteArticleAsyncTest()
        {
            var a1 = await ObjectHelper.CreateNewAsync();
            var a2 = await ObjectHelper.CreateNewAsync();
            var a3 = await ObjectHelper.CreateNewAsync();
            var a4 = await ObjectHelper.CreateNewAsync();

            await Articles.MultiDeleteAsync(a1.Type, a1.Id, a2.Id, a3.Id, a4.Id);
            var ids = new[] { a1.Id, a2.Id, a3.Id, a4.Id };
            for (int i = 0; i < ids.Length; i++)
            {
                try
                {
                    var copy = await Articles.GetAsync("object", ids[i]);
                    Assert.Fail("Operation should have faulted since the article has been deleted.");
                }
                catch (Net45.AppacitiveException ex)
                {
                    var msg = string.Format("Cannot locate article of type 'object' and id {0}.", ids[i]);
                    Assert.IsTrue(ex.Message == msg);
                }
            }

        }

        [TestMethod]
        public async Task DeleteArticleAsyncTest()
        {

            // Create the article
            var saved = await ObjectHelper.CreateNewAsync();

            // Delete the article
            await Articles.DeleteAsync("object", saved.Id);

            // Try and get and confirm that the article is deleted.
            try
            {
                var copy = await Articles.GetAsync("object", saved.Id);
                Assert.Fail("Operation should have faulted since the article has been deleted.");
            }
            catch (Net45.AppacitiveException ex)
            {
                var msg = string.Format("Cannot locate article of type 'object' and id {0}.", saved.Id);
                Assert.IsTrue(ex.Message == msg);
            }

        }

        [TestMethod]
        public async Task UpdateArticleWithNoUpdateAsyncTest()
        {
            var stopWatch = new System.Diagnostics.Stopwatch();

            // Create the article
            dynamic article = new Article("object");
            decimal pi = 22.0m / 7.0m;
            article.intfield = 1;
            article.decimalfield = pi;

            var saved = await ObjectHelper.CreateNewAsync(article as Article);
            var firstUpdateTime = saved.UtcLastUpdated;

            stopWatch.Start();

            //Dummy update, shouldn't make any api call, assuming api call takes atleast 50 ms
            await saved.SaveAsync();

            stopWatch.Stop();

            Assert.IsTrue(stopWatch.ElapsedMilliseconds < 50);
            Console.WriteLine(stopWatch.ElapsedMilliseconds);

            //Cleanup
            await Articles.DeleteAsync(saved.Type, saved.Id);
        }

        [TestMethod]
        public async Task UpdateArticlePropertyAsyncTest()
        {
            // Create the article
            dynamic article = new Article("object");
            decimal pi = 22.0m / 7.0m;
            article.intfield = 1;
            article.decimalfield = pi;
            var saved = await ObjectHelper.CreateNewAsync(article as Article);


            // Get the newly created article
            dynamic copy = await Articles.GetAsync("object", saved.Id);
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
            dynamic updated = await Articles.GetAsync("object", saved.Id);
            Assert.IsNotNull(updated);
            intfield = updated.intfield;
            decimalField = updated.decimalfield;
            string stringField = updated.stringfield;

            Assert.IsTrue(intfield == 2, "intfield not updated.");
            Assert.IsTrue(decimalField == 30, "decimal field not updated.");
            Assert.IsTrue(stringField == "Test", "stringfield not updated.");

        }

        [TestMethod]
        public async Task UpdateArticleTagAsyncTest()
        {
            string tagToRemove = "one";
            string tagPersist = "two";
            string tagToAdd = "three";

            // Create the article
            dynamic article = new Article("object");
            decimal pi = 22.0m / 7.0m;
            article.intfield = 1;
            article.decimalfield = pi;

            //Add tag
            article.AddTag(tagToRemove);
            article.AddTag(tagPersist);

            var saved = await ObjectHelper.CreateNewAsync(article as Article);

            // Get the newly created article
            var afterFirstUpdate = await Articles.GetAsync("object", saved.Id);
            Assert.IsNotNull(afterFirstUpdate);
            Assert.IsTrue(afterFirstUpdate.Tags.Count(tag => string.Equals(tag, tagPersist, StringComparison.OrdinalIgnoreCase)) == 1);
            Assert.IsTrue(afterFirstUpdate.Tags.Count(tag => string.Equals(tag, tagToRemove, StringComparison.OrdinalIgnoreCase)) == 1);
            Assert.IsTrue(afterFirstUpdate.Tags.Count() == 2);

            //Add/Remove tag
            afterFirstUpdate.RemoveTag(tagToRemove);
            afterFirstUpdate.AddTag(tagToAdd);
            await afterFirstUpdate.SaveAsync();

            var afterSecondUpdate = await Articles.GetAsync("object", saved.Id);

            Assert.IsTrue(afterSecondUpdate.Tags.Count(tag => string.Equals(tag, tagToRemove, StringComparison.OrdinalIgnoreCase)) == 0);
            Assert.IsTrue(afterSecondUpdate.Tags.Count(tag => string.Equals(tag, tagToAdd, StringComparison.OrdinalIgnoreCase)) == 1);
            Assert.IsTrue(afterSecondUpdate.Tags.Count() == 2);

            //Cleanup
            await Articles.DeleteAsync(afterSecondUpdate.Type, afterSecondUpdate.Id);
        }

        [TestMethod]
        public async Task UpdateArticleAttributeAsyncTest()
        {
            string attrToRemove = "one";
            string attrPersist = "two";
            string attrToAdd = "three";

            // Create the article
            dynamic article = new Article("object");
            decimal pi = 22.0m / 7.0m;
            article.intfield = 1;
            article.decimalfield = pi;

            //Add Attributes
            article.SetAttribute(attrToRemove, attrToRemove);
            article.SetAttribute(attrPersist, attrPersist);

            var saved = await ObjectHelper.CreateNewAsync(article as Article);

            // Get the newly created article
            var afterFirstUpdate = await Articles.GetAsync("object", saved.Id);
            Assert.IsNotNull(afterFirstUpdate);
            Assert.IsTrue(afterFirstUpdate.Attributes.Count(tag => string.Equals(tag.Key, attrPersist, StringComparison.OrdinalIgnoreCase)) == 1);
            Assert.IsTrue(afterFirstUpdate.Attributes.Count(tag => string.Equals(tag.Key, attrToRemove, StringComparison.OrdinalIgnoreCase)) == 1);
            Assert.IsTrue(afterFirstUpdate.Attributes.Count() == 2);

            //Add/Remove Attribute
            afterFirstUpdate.RemoveAttribute(attrToRemove);
            afterFirstUpdate.SetAttribute(attrToAdd, attrToAdd);
            await afterFirstUpdate.SaveAsync();

            var afterSecondUpdate = await Articles.GetAsync("object", saved.Id);

            Assert.IsTrue(afterSecondUpdate.Attributes.Count(tag => string.Equals(tag.Key, attrPersist, StringComparison.OrdinalIgnoreCase)) == 1);
            Assert.IsTrue(afterSecondUpdate.Attributes.Count(tag => string.Equals(tag.Key, attrToAdd, StringComparison.OrdinalIgnoreCase)) == 1);
            Assert.IsTrue(afterSecondUpdate.Attributes.Count() == 2);

            //Cleanup
            await Articles.DeleteAsync(afterSecondUpdate.Type, afterSecondUpdate.Id);
        }

        [TestMethod]
        public async Task FindAllArticlesAsyncTest()
        {
            // Create the article
            var saved = await ObjectHelper.CreateNewAsync();

            // Search
            var articles = await Articles.FindAllAsync("object");
            articles.ForEach(a => Console.WriteLine(a.Id));
            Console.WriteLine("page:{0} pageSize:{1} total: {2}", articles.PageNumber, articles.PageSize, articles.TotalRecords);

        }

        [TestMethod]
        public async Task FindAllArticlesAsyncWithQueryTest()
        {
            // Create the article
            dynamic article = new Article("object");
            article.stringfield = Unique.String;
            dynamic obj = await ObjectHelper.CreateNewAsync(article as Article);

            // Search
            string stringToSearch = obj.stringfield;
            var articles = await Articles.FindAllAsync("object", Query.Property("stringfield").IsEqualTo(stringToSearch).AsString());
            Assert.IsNotNull(articles);
            Assert.IsTrue(articles.Count == 1);
            Console.WriteLine("page:{0} pageSize:{1} total: {2}", articles.PageNumber, articles.PageSize, articles.TotalRecords);

        }

        [TestMethod]
        public async Task FindAllArticlesAsyncWithNestedQueryTest()
        {

            // Create the article
            dynamic article = new Article("object");
            article.stringfield = Unique.String;
            article.intfield = 10;
            dynamic obj = await ObjectHelper.CreateNewAsync(article as Article);

            // Search
            string stringToSearch = obj.stringfield;
            var query = BooleanOperator.And(new[] 
                        {
                            Query.Property("stringfield").IsEqualTo(stringToSearch),
                            Query.Property("intfield").IsEqualTo(10)
                        });

            var articles = await Articles.FindAllAsync("object", query.AsString());
            Assert.IsNotNull(articles);
            Assert.IsTrue(articles.Count == 1);
            Console.WriteLine("page:{0} pageSize:{1} total: {2}", articles.PageNumber, articles.PageSize, articles.TotalRecords);

        }

        [TestMethod]
        public async Task FindNonExistantPageTest()
        {
            // Search
            var articles = await Articles.FindAllAsync("object", Query.None, Article.AllFields, 10000, 500);
            Assert.IsNotNull(articles);
            Console.WriteLine("page:{0} pageSize:{1} total: {2}", articles.PageNumber, articles.PageSize, articles.TotalRecords);
        }

        [TestMethod]
        public async Task FindAndDisplayAllArticlesTest()
        {
            var waitHandle = new ManualResetEvent(false);

            // Create the article
            dynamic obj = new Article("object");
            obj.stringfield = Unique.String;
            await obj.SaveAsync();
            var saved = obj as Article;
            Console.WriteLine("Created articled with id {0}", saved.Id);
            var index = 1;
            // Search
            var articles = await Articles.FindAllAsync("object", Query.None, Article.AllFields, 1, 100);
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

        [TestMethod]
        public async Task GetConnectedArticlesAsyncTest()
        {

            // Create objects
            var obj1 = await ObjectHelper.CreateNewAsync();
            var obj2 = await ObjectHelper.CreateNewAsync();
            var obj3 = await ObjectHelper.CreateNewAsync();
            var obj4 = await ObjectHelper.CreateNewAsync();
            var obj5 = await ObjectHelper.CreateNewAsync();
            // Create connections
            await Connection.New("sibling").FromExistingArticle("object", obj1.Id).ToExistingArticle("object", obj2.Id).SaveAsync();
            await Connection.New("sibling").FromExistingArticle("object", obj1.Id).ToExistingArticle("object", obj3.Id).SaveAsync();
            await Connection.New("sibling").FromExistingArticle("object", obj1.Id).ToExistingArticle("object", obj4.Id).SaveAsync();
            await Connection.New("sibling").FromExistingArticle("object", obj1.Id).ToExistingArticle("object", obj5.Id).SaveAsync();
            // Get connected
            var connectedArticles = await obj1.GetConnectedArticlesAsync("sibling");
            Assert.IsTrue(connectedArticles != null);
            Assert.IsTrue(connectedArticles.TotalRecords == 4);
            Assert.IsTrue(connectedArticles.Select(x => x.Id).Intersect(new[] { obj2.Id, obj3.Id, obj4.Id, obj5.Id }).Count() == 4);
        }
    }
}
