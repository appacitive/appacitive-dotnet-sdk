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
        public async Task FindAllArticlesAsyncWithSpecialCharacterInQueryTest()
        {
            // Create the article
            dynamic article = new Article("object");
            article.stringfield = "129764_TouricoTGS_Museum of Modern Art and Casemates du Bock (tunnels in the city’s cliffs)";
                //Unique.String + "&" + "12las@";
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
        public async Task GetConnectedArticlesWithZeroConnectionsAsyncTest()
        {

            // Create objects
            var obj1 = await ObjectHelper.CreateNewAsync();
            // Get connected. Should return zero articles
            var connectedArticles = await obj1.GetConnectedArticlesAsync("sibling");
            Assert.IsTrue(connectedArticles != null);
            Assert.IsTrue(connectedArticles.TotalRecords == 0);
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


        [TestMethod]
        public async Task QueryArticleWithSingleQuotedValueTest()
        {
            dynamic obj = new Article("object");
            var stringValue = "Pan's Labyrinth" + Unique.String;
            obj.stringfield = stringValue;
            await obj.SaveAsync();

            PagedList<Article> result = await Articles.FindAllAsync("object", Query.Property("stringfield").IsEqualTo(stringValue).ToString());
            Assert.IsTrue(result.TotalRecords == 1, "Expected single record but multiple records were returned.");
            Assert.IsTrue(result.Single().Id == obj.Id);
        }

        [TestMethod]
        public async Task QueryWithTagsMatchAllTest()
        {
            // Create the test object.
            Article obj = new Article("object");
            var tags = new string[] { Unique.String, Unique.String };
            obj.Set<string>("stringfield", Unique.String);
            obj.AddTags(tags);
            await obj.SaveAsync();

            // Search for the object with tags.
            var matches = await Articles.FindAllAsync("object", Query.Tags.MatchAll(tags).ToString());
            Assert.IsTrue(matches != null);
            Assert.IsTrue(matches.Count == 1);
            Assert.IsTrue(matches[0] != null);
            Assert.IsTrue(matches[0].Id == obj.Id);
        }


        [TestMethod]
        public async Task QueryWithTagsMatchOneOrMoreTest()
        {
            var tag1 = Unique.String;
            var tag2 = Unique.String;
            // Create the test object 1.
            Article obj1 = new Article("object");
            obj1.Set<string>("stringfield", Unique.String);
            obj1.AddTag(tag1);
            await obj1.SaveAsync();

            Article obj2 = new Article("object");
            obj2.Set<string>("stringfield", Unique.String);
            obj2.AddTag(tag2);
            await obj2.SaveAsync();

            // Search for the object with tags.
            var matches = await Articles.FindAllAsync("object", Query.Tags.MatchOneOrMore(tag1, tag2).ToString());
            Assert.IsTrue(matches != null);
            Assert.IsTrue(matches.Count == 2);
            Assert.IsTrue(matches[0] != null && matches[1] != null );
            Assert.IsTrue(matches[0].Id == obj1.Id || matches[1].Id == obj1.Id);
            Assert.IsTrue(matches[0].Id == obj2.Id || matches[1].Id == obj2.Id);
        }

        [TestMethod]
        public async Task FreeTextSearchTest()
        {
            var value = Unique.String + " " + Unique.String;
            var obj = new Article("object");
            obj.Set<string>("stringfield", value);
            await obj.SaveAsync();

            var results = await Articles.FreeTextSearchAsync("object", value);
            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results[0].Id == obj.Id);
        }


        [TestMethod]
        public async Task FreeTextSearchWithMinusOperatorTest()
        {
            var mandatoryToken = Unique.String;
            var optionalToken = Unique.String;
            
            // Create one object with only the mandatory token.
            var obj1 = new Article("object");
            obj1.Set<string>("stringfield", mandatoryToken);
            

            // Create one object with the mandatory token and optional token.
            var obj2 = new Article("object");
            obj2.Set<string>("stringfield", mandatoryToken + " " + optionalToken);
            

            // Create one object with only optional token
            var obj3 = new Article("object");
            obj3.Set<string>("stringfield", optionalToken);

            await Task.WhenAll(obj1.SaveAsync(), obj2.SaveAsync(), obj3.SaveAsync());

            var results = await Articles.FreeTextSearchAsync("object", mandatoryToken + " -" + optionalToken);
            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results[0].Id == obj1.Id );
        }

        [TestMethod]
        public async Task FreeTextSearchWithQuestionMarkOperatorTest()
        {
            var prefix = Unique.String;
            var suffix = Unique.String;

            // Create one object with only the mandatory token.
            var obj1 = new Article("object");
            obj1.Set<string>("stringfield", prefix + "X" + suffix);


            // Create one object with the mandatory token and optional token.
            var obj2 = new Article("object");
            obj2.Set<string>("stringfield", prefix + "Y" + suffix);

            await Task.WhenAll(obj1.SaveAsync(), obj2.SaveAsync());

            var results = await Articles.FreeTextSearchAsync("object", prefix + "?" + suffix);
            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Count == 2);
            Assert.IsTrue(results[0].Id == obj1.Id || results[0].Id == obj2.Id);
            Assert.IsTrue(results[1].Id == obj1.Id || results[1].Id == obj2.Id);
        }

        [TestMethod]
        public async Task FreeTextSearchWithProximityOperatorTest()
        {
            var prefix = Unique.String;
            var suffix = Unique.String;

            // Create one object with only the mandatory token.
            var obj1 = new Article("object");
            obj1.Set<string>("stringfield", prefix + " word1" + " word2" + " word3 " + suffix);


            // Create one object with the mandatory token and optional token.
            var obj2 = new Article("object");
            obj2.Set<string>("stringfield", prefix + " word1" + " word2" + " word3" + " word4" + " word5 " + suffix);

            await Task.WhenAll(obj1.SaveAsync(), obj2.SaveAsync());

            var results = await Articles.FreeTextSearchAsync("object", "\"" + prefix + " " + suffix + "\"~4");
            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results[0].Id == obj1.Id);
        }

        [TestMethod]
        public async Task GetConnectedArticlesWithSortingSupportTest()
        {
            // Create 5 connected articles and request page 2 with page size of 2.
            // With sorting, it should return specific articles.
            var root = await ObjectHelper.CreateNewAsync();

            List<Article> children = new List<Article>();
            children.Add(ObjectHelper.NewInstance());
            children.Add(ObjectHelper.NewInstance());
            children.Add(ObjectHelper.NewInstance());
            children.Add(ObjectHelper.NewInstance());
            children.Add(ObjectHelper.NewInstance());

            var tasks = children.ConvertAll(x =>
                Connection.New("sibling").FromExistingArticle("object", root.Id).ToNewArticle("object", x).SaveAsync());
            await Task.WhenAll(tasks);

            children = children.OrderBy(x => x.Id).ToList();
            var results = await root.GetConnectedArticlesAsync("sibling", orderBy: "__id", sortOrder: SortOrder.Ascending,
                pageSize:2, pageNumber: 2);
            Assert.IsTrue(results.Count == 2);
            Assert.IsTrue(results[0].Id == children[2].Id);
            Assert.IsTrue(results[1].Id == children[3].Id);
        }

        [TestMethod]
        public async Task ArticleUpdateWithVersioningMvccTest()
        {
            var article = await ObjectHelper.CreateNewAsync();
            // This should work
            article.Set<string>("stringfield", Unique.String);
            await article.SaveAsync();
            // This should fail as I am trying to update with an older revision
            article.Set<string>("stringfield", Unique.String);
            bool isFault = false;
            try
            {
                await article.SaveAsync(article.Revision - 1);
            }
            catch (Net45.AppacitiveException ex)
            {
                if (ex.Code == "14008")
                    isFault = true;
                else Assert.Fail(ex.Message);
            }
            if (isFault == false) 
                Assert.Fail("No fault was raised on a bad revision update.");
        }
    }
}
