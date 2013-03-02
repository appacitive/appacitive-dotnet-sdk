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
        public async Task GetArticleAsyncTest()
        {
            // Create new article
            dynamic article = new Article("object");
            decimal pi = 22.0m/7.0m;
            article.intfield = 1;
            article.decimalfield = pi;
            var saved = await ObjectHelper.CreateNewAsync( article as Article );

            // Get the created article
            dynamic copy = await Article.GetAsync("object", saved.Id);
            Assert.IsNotNull(copy);
            int intfield = copy.intfield;
            decimal decimalField = copy.decimalfield;
            Assert.IsTrue(intfield == 1);
            Assert.IsTrue(decimalField == pi);

        }

        [TestMethod]
        public async Task DeleteArticleAsyncTest()
        {

            // Create the article
            var saved = await ObjectHelper.CreateNewAsync();

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

        [TestMethod]
        public async Task UpdateArticleAsyncTest()
        {
            // Create the article
            dynamic article = new Article("object");
            decimal pi = 22.0m / 7.0m;
            article.intfield = 1;
            article.decimalfield = pi;
            var saved = await ObjectHelper.CreateNewAsync(article as Article);
            

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


        [TestMethod]
        public async Task FindAllArticlesAsyncTest()
        {
            // Create the article
            var saved = await ObjectHelper.CreateNewAsync();

            // Search
            var articles = await Article.FindAllAsync("object");
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
            var articles = await Article.FindAllAsync("object", Query.Property("stringfield").IsEqualTo(stringToSearch).AsString());
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

            var articles = await Article.FindAllAsync("object", query.AsString());
            Assert.IsNotNull(articles);
            Assert.IsTrue(articles.Count == 1);
            Console.WriteLine("page:{0} pageSize:{1} total: {2}", articles.PageNumber, articles.PageSize, articles.TotalRecords);

        }

        [TestMethod]
        public async Task FindNonExistantPageTest()
        {
            // Search
            var articles = await Article.FindAllAsync("object", Query.None, Article.AllFields, 10000, 500);
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
            var articles = await Article.FindAllAsync("object", Query.None, Article.AllFields, 1, 100);
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
            await Connection.Create("sibling").FromExistingArticle("object", obj1.Id).ToExistingArticle("object", obj2.Id).SaveAsync();
            await Connection.Create("sibling").FromExistingArticle("object", obj1.Id).ToExistingArticle("object", obj3.Id).SaveAsync();
            await Connection.Create("sibling").FromExistingArticle("object", obj1.Id).ToExistingArticle("object", obj4.Id).SaveAsync();
            await Connection.Create("sibling").FromExistingArticle("object", obj1.Id).ToExistingArticle("object", obj5.Id).SaveAsync();
            // Get connected
            var connectedArticles = await obj1.GetConnectedArticlesAsync("sibling");
            Assert.IsTrue(connectedArticles != null);
            Assert.IsTrue(connectedArticles.TotalRecords == 4);
            Assert.IsTrue(connectedArticles.Select(x => x.Id).Intersect(new[] { obj2.Id, obj3.Id, obj4.Id, obj5.Id }).Count() == 4);

        }
        
    }
}
