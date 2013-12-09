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
    public class ObjectFixture
    {
        [TestMethod]
        public async Task CreateObjectAsyncTest()
        {
            dynamic obj = new APObject("object");
            obj.intfield = 1;
            obj.decimalfield = 22m / 7m;
            await obj.SaveAsync();
            var saved = obj as APObject;
            Assert.IsNotNull(saved);
            Assert.IsTrue(string.IsNullOrWhiteSpace(saved.Id) == false);
            Assert.IsTrue(saved.Type == "object");
            Assert.IsTrue(saved.Revision == 1);
            Assert.IsTrue(saved.CreatedAt.Subtract(DateTime.Now).Duration().Seconds < 15);
            Assert.IsTrue(saved.LastUpdatedAt.Subtract(DateTime.Now).Duration().Seconds < 15);
            Console.WriteLine("Created apObject with id {0}.", saved.Id);
        }

        [TestMethod]
        public async Task GetDevicesShouldReturnDeviceObjectsTest()
        {
            var created = await DeviceHelper.CreateNewAsync();
            var devices = await APObjects.FindAllAsync("device");
            Assert.IsFalse(devices.Exists(d => d is APDevice == false));
        }

        [TestMethod]
        public async Task GetUsersShouldReturnUserObjectsTest()
        {
            var created = await UserHelper.CreateNewUserAsync();
            var users = await APObjects.FindAllAsync("user");
            Assert.IsFalse(users.Exists(d => d is APUser == false));
        }

        [TestMethod]
        public async Task GetObjectAsyncTest()
        {
            // Create new object
            dynamic obj = new APObject("object");
            decimal pi = 22.0m / 7.0m;
            obj.intfield = 1;
            obj.decimalfield = pi;
            var saved = await ObjectHelper.CreateNewAsync(obj as APObject);

            // Get the created object
            dynamic copy = await APObjects.GetAsync("object", saved.Id);
            Assert.IsNotNull(copy);
            int intfield = copy.intfield;
            decimal decimalField = copy.decimalfield;
            Assert.IsTrue(intfield == 1);
            Assert.IsTrue(decimalField == pi);
            Assert.IsTrue(copy.Type == "object");
            Assert.IsTrue(copy.Revision == 1);
            Assert.IsTrue(copy.CreatedAt.Subtract(DateTime.Now).Duration().Seconds < 15);
            Assert.IsTrue(copy.LastUpdatedAt.Subtract(DateTime.Now).Duration().Seconds < 15);

        }

        [TestMethod]
        public async Task MultiValueObjectTest()
        {
            var obj = new APObject("object");
            obj.SetList<string>("multifield", new[] { "1", "2", "3", "4" });
            await obj.SaveAsync();

            var read = await APObjects.GetAsync("object", obj.Id);
            var value = read.GetList<string>("multifield");
            var strList = read.GetList<string>("multifield");
            var intList = read.GetList<int>("multifield");
        }

        [TestMethod]
        public async Task MultiGetObjectAsyncTest()
        {
            // Create new object
            var obj1 = await ObjectHelper.CreateNewAsync();
            var obj2 = await ObjectHelper.CreateNewAsync();

            // Get the created objects
            var enumerable = await APObjects.MultiGetAsync("object", new[] { obj1.Id, obj2.Id });

            // Asserts
            Assert.IsNotNull(enumerable);
            var list = enumerable.Select(x => x.Id);
            Assert.IsTrue(list.Intersect(new[] { obj1.Id, obj2.Id }).Count() == 2);

        }

        [TestMethod]
        public async Task BulkDeleteObjectAsyncTest()
        {
            var a1 = await ObjectHelper.CreateNewAsync();
            var a2 = await ObjectHelper.CreateNewAsync();
            var a3 = await ObjectHelper.CreateNewAsync();
            var a4 = await ObjectHelper.CreateNewAsync();

            await APObjects.MultiDeleteAsync(a1.Type, a1.Id, a2.Id, a3.Id, a4.Id);
            var ids = new[] { a1.Id, a2.Id, a3.Id, a4.Id };
            for (int i = 0; i < ids.Length; i++)
            {
                try
                {
                    var copy = await APObjects.GetAsync("object", ids[i]);
                    Assert.Fail("Operation should have faulted since the object has been deleted.");
                }
                catch (AppacitiveException)
                {   
                }
            }

        }

        [TestMethod]
        public async Task DeleteObjectAsyncTest()
        {

            // Create the object
            var saved = await ObjectHelper.CreateNewAsync();

            // Delete the object
            await APObjects.DeleteAsync("object", saved.Id);

            // Try and get and confirm that the object is deleted.
            try
            {
                var copy = await APObjects.GetAsync("object", saved.Id);
                Assert.Fail("Operation should have faulted since the object has been deleted.");
            }
            catch (AppacitiveException)
            {   
            }

        }

        [TestMethod]
        public async Task UpdateObjectWithNoUpdateAsyncTest()
        {
            var stopWatch = new System.Diagnostics.Stopwatch();

            // Create the object
            dynamic obj = new APObject("object");
            decimal pi = 22.0m / 7.0m;
            obj.intfield = 1;
            obj.decimalfield = pi;

            var saved = await ObjectHelper.CreateNewAsync(obj as APObject);
            var firstUpdateTime = saved.LastUpdatedAt;

            stopWatch.Start();

            //Dummy update, shouldn't make any api call, assuming api call takes atleast 50 ms
            await saved.SaveAsync();

            stopWatch.Stop();

            Assert.IsTrue(stopWatch.ElapsedMilliseconds < 50);
            Console.WriteLine(stopWatch.ElapsedMilliseconds);

            //Cleanup
            await APObjects.DeleteAsync(saved.Type, saved.Id);
        }

        [TestMethod]
        public async Task UpdateObjectPropertyAsyncTest()
        {
            // Create the object
            dynamic obj = new APObject("object");
            decimal pi = 22.0m / 7.0m;
            obj.intfield = 1;
            obj.decimalfield = pi;
            var saved = await ObjectHelper.CreateNewAsync(obj as APObject);


            // Get the newly created object
            dynamic copy = await APObjects.GetAsync("object", saved.Id);
            Assert.IsNotNull(copy);
            int intfield = copy.intfield;
            decimal decimalField = copy.decimalfield;
            Assert.IsTrue(intfield == 1);
            Assert.IsTrue(decimalField == pi);

            // Update the object
            copy.intfield = 2;
            copy.decimalfield = 30m;
            copy.stringfield = "Test";
            await copy.SaveAsync();

            // Get updated copy and verify
            dynamic updated = await APObjects.GetAsync("object", saved.Id);
            Assert.IsNotNull(updated);
            intfield = updated.intfield;
            decimalField = updated.decimalfield;
            string stringField = updated.stringfield;

            Assert.IsTrue(intfield == 2, "intfield not updated.");
            Assert.IsTrue(decimalField == 30, "decimal field not updated.");
            Assert.IsTrue(stringField == "Test", "stringfield not updated.");

        }

        [TestMethod]
        public async Task UpdateObjectTagAsyncTest()
        {
            string tagToRemove = "one";
            string tagPersist = "two";
            string tagToAdd = "three";

            // Create the object
            dynamic obj = new APObject("object");
            decimal pi = 22.0m / 7.0m;
            obj.intfield = 1;
            obj.decimalfield = pi;

            //Add tag
            obj.AddTag(tagToRemove);
            obj.AddTag(tagPersist);

            var saved = await ObjectHelper.CreateNewAsync(obj as APObject);

            // Get the newly created object
            var afterFirstUpdate = await APObjects.GetAsync("object", saved.Id);
            Assert.IsNotNull(afterFirstUpdate);
            Assert.IsTrue(afterFirstUpdate.Tags.Count(tag => string.Equals(tag, tagPersist, StringComparison.OrdinalIgnoreCase)) == 1);
            Assert.IsTrue(afterFirstUpdate.Tags.Count(tag => string.Equals(tag, tagToRemove, StringComparison.OrdinalIgnoreCase)) == 1);
            Assert.IsTrue(afterFirstUpdate.Tags.Count() == 2);

            //Add/Remove tag
            afterFirstUpdate.RemoveTag(tagToRemove);
            afterFirstUpdate.AddTag(tagToAdd);
            await afterFirstUpdate.SaveAsync();

            var afterSecondUpdate = await APObjects.GetAsync("object", saved.Id);

            Assert.IsTrue(afterSecondUpdate.Tags.Count(tag => string.Equals(tag, tagToRemove, StringComparison.OrdinalIgnoreCase)) == 0);
            Assert.IsTrue(afterSecondUpdate.Tags.Count(tag => string.Equals(tag, tagToAdd, StringComparison.OrdinalIgnoreCase)) == 1);
            Assert.IsTrue(afterSecondUpdate.Tags.Count() == 2);

            //Cleanup
            await APObjects.DeleteAsync(afterSecondUpdate.Type, afterSecondUpdate.Id);
        }

        [TestMethod]
        public async Task UpdateObjectAttributeAsyncTest()
        {
            string attrToRemove = "one";
            string attrPersist = "two";
            string attrToAdd = "three";

            // Create the object
            dynamic obj = new APObject("object");
            decimal pi = 22.0m / 7.0m;
            obj.intfield = 1;
            obj.decimalfield = pi;

            //Add Attributes
            obj.SetAttribute(attrToRemove, attrToRemove);
            obj.SetAttribute(attrPersist, attrPersist);

            var saved = await ObjectHelper.CreateNewAsync(obj as APObject);

            // Get the newly created object
            var afterFirstUpdate = await APObjects.GetAsync("object", saved.Id);
            Assert.IsNotNull(afterFirstUpdate);
            Assert.IsTrue(afterFirstUpdate.Attributes.Count(tag => string.Equals(tag.Key, attrPersist, StringComparison.OrdinalIgnoreCase)) == 1);
            Assert.IsTrue(afterFirstUpdate.Attributes.Count(tag => string.Equals(tag.Key, attrToRemove, StringComparison.OrdinalIgnoreCase)) == 1);
            Assert.IsTrue(afterFirstUpdate.Attributes.Count() == 2);

            //Add/Remove Attribute
            afterFirstUpdate.RemoveAttribute(attrToRemove);
            afterFirstUpdate.SetAttribute(attrToAdd, attrToAdd);
            await afterFirstUpdate.SaveAsync();

            var afterSecondUpdate = await APObjects.GetAsync("object", saved.Id);

            Assert.IsTrue(afterSecondUpdate.Attributes.Count(tag => string.Equals(tag.Key, attrPersist, StringComparison.OrdinalIgnoreCase)) == 1);
            Assert.IsTrue(afterSecondUpdate.Attributes.Count(tag => string.Equals(tag.Key, attrToAdd, StringComparison.OrdinalIgnoreCase)) == 1);
            Assert.IsTrue(afterSecondUpdate.Attributes.Count() == 2);

            //Cleanup
            await APObjects.DeleteAsync(afterSecondUpdate.Type, afterSecondUpdate.Id);
        }

        [TestMethod]
        public async Task FindAllObjectsAsyncTest()
        {
            // Create the object
            var saved = await ObjectHelper.CreateNewAsync();

            // Search
            var objects = await APObjects.FindAllAsync("object");
            objects.ForEach(a => Console.WriteLine(a.Id));
            Console.WriteLine("page:{0} pageSize:{1} total: {2}", objects.PageNumber, objects.PageSize, objects.TotalRecords);

        }

        [TestMethod]
        public async Task FindAllObjectsAsyncWithQueryTest()
        {
            // Create the object
            dynamic apObject = new APObject("object");
            apObject.stringfield = Unique.String;
            dynamic obj = await ObjectHelper.CreateNewAsync(apObject as APObject);

            // Search
            string stringToSearch = obj.stringfield;
            var objects = await APObjects.FindAllAsync("object", Query.Property("stringfield").IsEqualTo(stringToSearch));
            Assert.IsNotNull(objects);
            Assert.IsTrue(objects.Count == 1);
            Console.WriteLine("page:{0} pageSize:{1} total: {2}", objects.PageNumber, objects.PageSize, objects.TotalRecords);

        }

        [TestMethod]
        public async Task FindAllObjectsAsyncWithSpecialCharacterInQueryTest()
        {
            // Create the object
            dynamic apObject = new APObject("object");
            apObject.stringfield = Unique.String + " 129764_TouricoTGS_Museum (tunnels in the city’s cliffs)";
                //Unique.String + "&" + "12las@";
            dynamic obj = await ObjectHelper.CreateNewAsync(apObject as APObject);

            // Search
            string stringToSearch = obj.stringfield;
            var objects = await APObjects.FindAllAsync("object", Query.Property("stringfield").IsEqualTo(stringToSearch));
            Assert.IsNotNull(objects);
            Assert.IsTrue(objects.Count == 1);
            Console.WriteLine("page:{0} pageSize:{1} total: {2}", objects.PageNumber, objects.PageSize, objects.TotalRecords);

        }

        [TestMethod]
        public async Task FindAllObjectsAsyncWithNestedQueryTest()
        {

            // Create the object
            dynamic apObject = new APObject("object");
            apObject.stringfield = Unique.String;
            apObject.intfield = 10;
            dynamic obj = await ObjectHelper.CreateNewAsync(apObject as APObject);

            // Search
            string stringToSearch = obj.stringfield;
            var query = Query.And(new[] 
                        {
                            Query.Property("stringfield").IsEqualTo(stringToSearch),
                            Query.Property("intfield").IsEqualTo(10)
                        });

            var objects = await APObjects.FindAllAsync("object", query);
            Assert.IsNotNull(objects);
            Assert.IsTrue(objects.Count == 1);
            Console.WriteLine("page:{0} pageSize:{1} total: {2}", objects.PageNumber, objects.PageSize, objects.TotalRecords);

        }

        [TestMethod]
        public async Task FindNonExistantPageTest()
        {
            // Search
            var objects = await APObjects.FindAllAsync("object", Query.None, APObject.AllFields, 10000, 500);
            Assert.IsNotNull(objects);
            Console.WriteLine("page:{0} pageSize:{1} total: {2}", objects.PageNumber, objects.PageSize, objects.TotalRecords);
        }

        [TestMethod]
        public async Task FindAndDisplayAllObjectsTest()
        {
            var waitHandle = new ManualResetEvent(false);

            // Create the object
            dynamic obj = new APObject("object");
            obj.stringfield = Unique.String;
            await obj.SaveAsync();
            var saved = obj as APObject;
            Console.WriteLine("Created apObj with id {0}", saved.Id);
            var index = 1;
            // Search
            var objects = await APObjects.FindAllAsync("object", Query.None, APObject.AllFields, 1, 100);
            do
            {
                objects.ForEach(a => Console.WriteLine("{0}) {1}", index++, a.Id));
                Console.WriteLine("page:{0} pageSize:{1} total: {2}", objects.PageNumber, objects.PageSize, objects.TotalRecords);
                if (objects.IsLastPage == false)
                    objects = await objects.NextPageAsync();
                else
                    break;
            } while (true);
            Console.WriteLine("Finished.");
        }

        [TestMethod]
        public async Task GetConnectedObjectsWithZeroConnectionsAsyncTest()
        {

            // Create objects
            var obj1 = await ObjectHelper.CreateNewAsync();
            // Get connected. Should return zero objects
            var connectedObjects = await obj1.GetConnectedObjectsAsync("sibling");
            Assert.IsTrue(connectedObjects != null);
            Assert.IsTrue(connectedObjects.TotalRecords == 0);
        }

        [TestMethod]
        public async Task GetConnectedObjectsAsyncTest()
        {

            // Create objects
            var obj1 = await ObjectHelper.CreateNewAsync();
            var obj2 = await ObjectHelper.CreateNewAsync();
            var obj3 = await ObjectHelper.CreateNewAsync();
            var obj4 = await ObjectHelper.CreateNewAsync();
            var obj5 = await ObjectHelper.CreateNewAsync();
            // Create connections
            await APConnection.New("sibling").FromExistingObject("object", obj1.Id).ToExistingObject("object", obj2.Id).SaveAsync();
            await APConnection.New("sibling").FromExistingObject("object", obj1.Id).ToExistingObject("object", obj3.Id).SaveAsync();
            await APConnection.New("sibling").FromExistingObject("object", obj1.Id).ToExistingObject("object", obj4.Id).SaveAsync();
            await APConnection.New("sibling").FromExistingObject("object", obj1.Id).ToExistingObject("object", obj5.Id).SaveAsync();
            // Get connected
            var connectedObjects = await obj1.GetConnectedObjectsAsync("sibling");
            Assert.IsTrue(connectedObjects != null);
            Assert.IsTrue(connectedObjects.TotalRecords == 4);
            Assert.IsTrue(connectedObjects.Select(x => x.Id).Intersect(new[] { obj2.Id, obj3.Id, obj4.Id, obj5.Id }).Count() == 4);
        }


        [TestMethod]
        public async Task QueryObjectWithSingleQuotedValueTest()
        {
            dynamic obj = new APObject("object");
            var stringValue = "Pan's Labyrinth" + Unique.String;
            obj.stringfield = stringValue;
            await obj.SaveAsync();

            PagedList<APObject> result = await APObjects.FindAllAsync("object", Query.Property("stringfield").IsEqualTo(stringValue));
            Assert.IsTrue(result.TotalRecords == 1, "Expected single record but multiple records were returned.");
            Assert.IsTrue(result.Single().Id == obj.Id);
        }

        [TestMethod]
        public async Task QueryWithTagsMatchAllTest()
        {
            // Create the test object.
            APObject obj = new APObject("object");
            var tags = new string[] { Unique.String, Unique.String };
            obj.Set<string>("stringfield", Unique.String);
            obj.AddTags(tags);
            await obj.SaveAsync();

            // Search for the object with tags.
            var matches = await APObjects.FindAllAsync("object", Query.Tags.MatchAll(tags));
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
            APObject obj1 = new APObject("object");
            obj1.Set<string>("stringfield", Unique.String);
            obj1.AddTag(tag1);
            await obj1.SaveAsync();

            APObject obj2 = new APObject("object");
            obj2.Set<string>("stringfield", Unique.String);
            obj2.AddTag(tag2);
            await obj2.SaveAsync();

            // Search for the object with tags.
            var matches = await APObjects.FindAllAsync("object", Query.Tags.MatchOneOrMore(tag1, tag2));
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
            var obj = new APObject("object");
            obj.Set<string>("stringfield", value);
            await obj.SaveAsync();

            var results = await APObjects.FreeTextSearchAsync("object", value);
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
            var obj1 = new APObject("object");
            obj1.Set<string>("stringfield", mandatoryToken);
            

            // Create one object with the mandatory token and optional token.
            var obj2 = new APObject("object");
            obj2.Set<string>("stringfield", mandatoryToken + " " + optionalToken);
            

            // Create one object with only optional token
            var obj3 = new APObject("object");
            obj3.Set<string>("stringfield", optionalToken);

            await Task.WhenAll(obj1.SaveAsync(), obj2.SaveAsync(), obj3.SaveAsync());

            var results = await APObjects.FreeTextSearchAsync("object", mandatoryToken + " -" + optionalToken);
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
            var obj1 = new APObject("object");
            obj1.Set<string>("stringfield", prefix + "X" + suffix);


            // Create one object with the mandatory token and optional token.
            var obj2 = new APObject("object");
            obj2.Set<string>("stringfield", prefix + "Y" + suffix);

            await Task.WhenAll(obj1.SaveAsync(), obj2.SaveAsync());

            var results = await APObjects.FreeTextSearchAsync("object", prefix + "?" + suffix);
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
            var obj1 = new APObject("object");
            obj1.Set<string>("stringfield", prefix + " word1" + " word2" + " word3 " + suffix);


            // Create one object with the mandatory token and optional token.
            var obj2 = new APObject("object");
            obj2.Set<string>("stringfield", prefix + " word1" + " word2" + " word3" + " word4" + " word5 " + suffix);

            await Task.WhenAll(obj1.SaveAsync(), obj2.SaveAsync());

            var results = await APObjects.FreeTextSearchAsync("object", "\"" + prefix + " " + suffix + "\"~4");
            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results[0].Id == obj1.Id);
        }

        [TestMethod]
        public async Task GetConnectedObjectsWithSortingSupportTest()
        {
            // Create 5 connected objects and request page 2 with page size of 2.
            // With sorting, it should return specific objects.
            var root = await ObjectHelper.CreateNewAsync();

            List<APObject> children = new List<APObject>();
            children.Add(ObjectHelper.NewInstance());
            children.Add(ObjectHelper.NewInstance());
            children.Add(ObjectHelper.NewInstance());
            children.Add(ObjectHelper.NewInstance());
            children.Add(ObjectHelper.NewInstance());

            var tasks = children.ConvertAll(x =>
                APConnection.New("sibling").FromExistingObject("object", root.Id).ToNewObject("object", x).SaveAsync());
            await Task.WhenAll(tasks);

            children = children.OrderBy(x => x.Id).ToList();
            var results = await root.GetConnectedObjectsAsync("sibling", orderBy: "__id", sortOrder: SortOrder.Ascending,
                pageSize:2, pageNumber: 2);
            Assert.IsTrue(results.Count == 2);
            Assert.IsTrue(results[0].Id == children[2].Id);
            Assert.IsTrue(results[1].Id == children[3].Id);
        }

        [TestMethod]
        public async Task ObjectUpdateWithVersioningMvccTest()
        {
            var obj = await ObjectHelper.CreateNewAsync();
            // This should work
            obj.Set<string>("stringfield", Unique.String);
            await obj.SaveAsync();
            // This should fail as I am trying to update with an older revision
            obj.Set<string>("stringfield", Unique.String);
            bool isFault = false;
            try
            {
                await obj.SaveAsync(obj.Revision - 1);
            }
            catch (AppacitiveException ex)
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
