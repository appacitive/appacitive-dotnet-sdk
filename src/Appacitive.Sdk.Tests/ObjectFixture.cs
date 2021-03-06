﻿using System;
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
        public async Task PropertyNotInStringTest()
        {
            var value = Unique.String;
            var obj = new APObject("object");
            obj.Set("stringfield", value);
            await obj.SaveAsync();
            var objects = await APObjects.FindAllAsync("object", Query.Property("stringfield").IsNotIn(new[] { Unique.String }), orderBy: "__id", sortOrder: SortOrder.Descending, pageSize: 1);
            Assert.AreEqual(obj.Id, objects.Single().Id);
            objects = await APObjects.FindAllAsync("object", Query.Property("stringfield").IsNotIn(new [] { value }), orderBy: "__id", sortOrder: SortOrder.Descending, pageSize: 1);
            Assert.AreNotEqual(obj.Id, objects.Single().Id);
        }

        [TestMethod]
        public async Task PropertyNotInIntegerTest()
        {
            var value = DateTime.UtcNow.Ticks;
            var obj = new APObject("object");
            obj.Set("intfield", value);
            await obj.SaveAsync();
            var objects = await APObjects.FindAllAsync("object", Query.Property("intfield").IsNotIn(new[] { 100L }), orderBy: "__id", sortOrder: SortOrder.Descending, pageSize: 1);
            Assert.AreEqual(obj.Id, objects.Single().Id);
            objects = await APObjects.FindAllAsync("object", Query.Property("intfield").IsNotIn(new[] { value }), orderBy: "__id", sortOrder: SortOrder.Descending, pageSize: 1);
            Assert.AreNotEqual(obj.Id, objects.Single().Id);
        }


        [TestMethod]
        public async Task IsNotNullQueryTest()
        {
            var obj = await ObjectHelper.CreateNewAsync();
            var objects = await APObjects.FindAllAsync("object", Query.Property("intfield").IsNotNull(), orderBy: "__id", sortOrder: SortOrder.Descending, pageSize: 1);
            Assert.AreEqual(obj.Id, objects.Single().Id);
        }

        [TestMethod]
        public async Task AggregateGetTest()
        {
            var objA = new APObject("object");
            objA.Set("decimalfield", 100.0m);
            var root = new APObject("object");
            root.Set("decimalfield", 50m);
            await APConnection
                .New("sibling")
                .FromNewObject("object", root)
                .ToNewObject("object", objA)
                .SaveAsync();
            await Task.Delay(20000);
            var rootCopy = await APObjects.GetAsync("object", root.Id);
            Assert.AreEqual(100.0m, rootCopy.GetAggregate("decimal_aggregate"));
        }

        

        [TestMethod]
        public async Task FindObjectsWithInQueryTest()
        {   
            var value = Unique.String;
            var queryValues = new[] { Unique.String, value, Unique.String, Unique.String };
            var obj = new APObject("object");
            obj.Set("stringfield", value);
            obj = await ObjectHelper.CreateNewAsync(obj);
            var results = await APObjects.FindAllAsync("object", Query.Property("stringfield").IsIn(queryValues), sortOrder: SortOrder.Descending, orderBy: "__id", pageSize: 5);
            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results.Single().Id == obj.Id);
        }

        [TestMethod]
        public async Task FindObjectsWithInWithIntegerQueryTest()
        {
            var value = Unique.String;
            long[] queryValues = new long[] { 1,2,3 };
            var obj = new APObject("object");
            obj.Set("intfield", 1);
            obj = await ObjectHelper.CreateNewAsync(obj);
            var results = await APObjects.FindAllAsync("object", Query.Property("intfield").IsIn(queryValues), sortOrder: SortOrder.Descending, orderBy: "__id", pageSize: 5);
            Assert.IsTrue(results.Exists(x => x.Id == obj.Id));
        }

        [TestMethod]
        public async Task FindObjectsWithInQueryForMultiValuedPropertyTest()
        {
            var value = Unique.String;
            var queryValues = new[] { Unique.String, value, Unique.String, Unique.String };
            var obj = new APObject("object");
            obj.SetList("multifield", new [] { value, Unique.String });
            obj = await ObjectHelper.CreateNewAsync(obj);
            var results = await APObjects.FindAllAsync("object", Query.Property("multifield").IsIn(queryValues), sortOrder: SortOrder.Descending, orderBy: "__id", pageSize: 5);
            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results.Single().Id == obj.Id);
        }

        [TestMethod]
        public async Task FindObjectsWithNullPropertiesTest()
        {
            var obj = new APObject("object");
            obj = await ObjectHelper.CreateNewAsync(obj);
            var results = await APObjects.FindAllAsync("object", Query.Property("stringfield").IsNull(), sortOrder: SortOrder.Descending, orderBy: "__id", pageSize: 5);
            Assert.IsTrue( results.Exists( x => x.Id ==obj.Id));
        }

        [TestMethod]
        public async Task FindObjectsWithNullAttributesTest()
        {
            var obj = new APObject("object");
            obj = await ObjectHelper.CreateNewAsync(obj);
            var results = await APObjects.FindAllAsync("object", Query.Attribute("stringfield").IsNull(), sortOrder: SortOrder.Descending, orderBy: "__id", pageSize: 5);
            Assert.IsTrue(results.Exists(x => x.Id == obj.Id));
        }

        [TestMethod]
        public async Task ForceUpdateEmptyShouldUpdateNothingTest()
        {
            var obj = await ObjectHelper.CreateNewAsync();
            var copy = new APObject("object", obj.Id);
            await copy.SaveAsync(forceUpdate: true);
            var existingProperties = obj.Properties.ToList();
            var updateProperties = copy.Properties.ToList();
            
            var existing = existingProperties
                                .Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString()))
                                .ToList();
            var updated = updateProperties.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString())).ToList();
            Assert.IsTrue(updated.Count == existing.Count);
        }


        [TestMethod]
        public async Task StartsWithAndEndsWithFiltersTest()
        {
            var prefix = Unique.String;
            var suffix = Unique.String;
            var obj = new APObject("object");
            obj.Set<string>("stringfield", prefix + suffix);
            await obj.SaveAsync();

            var found = (await APObjects.FindAllAsync("object", Query.Property("stringfield").StartsWith(prefix))).SingleOrDefault();
            Assert.IsTrue(found != null);
            Assert.IsTrue(found.Id == obj.Id);

            found = (await APObjects.FindAllAsync("object", Query.Property("stringfield").EndsWith(suffix))).SingleOrDefault();
            Assert.IsTrue(found != null);
            Assert.IsTrue(found.Id == obj.Id);
        }

        [TestMethod]
        public async Task ForceUpdateTest()
        {
            var obj = await ObjectHelper.CreateNewAsync();
            obj.Set("stringfield", Unique.String);
            await obj.SaveAsync();
            Assert.IsTrue(obj.Revision == 2);
            await obj.SaveAsync();
            Assert.IsTrue(obj.Revision == 2);
            await obj.SaveAsync(forceUpdate: true);
            Assert.IsTrue(obj.Revision == 3);
        }

        [TestMethod]
        public async Task RefreshAsyncTest()
        {
            var obj = await ObjectHelper.CreateNewAsync();
            var copy = await APObjects.GetAsync("object", obj.Id);
            var updatedValue = Unique.String;
            copy.Set("stringfield", updatedValue);
            await copy.SaveAsync();

            Assert.AreNotEqual(obj.Get<string>("stringfield"), updatedValue);
            await obj.RefreshAsync();
            Assert.AreEqual(obj.Get<string>("stringfield"), updatedValue);

        }


        [TestMethod]
        public async Task TypeMappingTest()
        {
            Score s1 = new Score { Points = 100 };
            s1.Badges.Add("novice");
            s1.Badges.Add("alcolyte");
            await s1.SaveAsync();
            Score s2 = new Score { Points = 100 };
            await s2.SaveAsync();
            AppContext.Types.MapObjectType<Score>("score");
            var saved = await APObjects.GetAsync("score", s1.Id);
            Assert.IsNotNull(saved);
            Assert.IsTrue(saved is Score);
            Assert.IsTrue(((Score)saved).Badges.SequenceEqual(new [] { "novice", "alcolyte" }));
            var scores = await APObjects.FindAllAsync("score");
            foreach (var score in scores)
            {
                Assert.IsNotNull(score);
                Assert.IsTrue(score is Score);
            }
        }


        [TestMethod]
        public async Task AddItemsToMultiValuedFieldAsyncTest()
        {
            var obj = new APObject("object");
            await obj.SaveAsync();
            await obj.AddItemsAsync("multifield", "1", "2", "3");
            var list = obj.GetList<string>("multifield");
            Assert.IsTrue(list.Except(new[] { "1", "2", "3" }).Count() == 0);
        }



        [TestMethod]
        public async Task AddUniqueItemsToMultiValuedFieldAsyncTest()
        {
            var obj = new APObject("object");
            obj.SetList("multifield", new int[] { 1, 2, 3 });
            await obj.SaveAsync();
            await obj.AddItemsAsync("multifield", true, "1", "2", "3", "4", "5");
            var list = obj.GetList<string>("multifield");
            Assert.IsTrue(list.Count() == 5);
            Assert.IsTrue(list.Except(new[] { "1", "2", "3", "4", "5" }).Count() == 0);
        }

        [TestMethod]
        public async Task RemoveItemsFromMultiValuedFieldAsyncTest()
        {
            var obj = new APObject("object");
            obj.SetList("multifield", new int[] { 1, 2, 3 });
            await obj.SaveAsync();
            await obj.RemoveItemsAsync("multifield", "1", "2");
            var list = obj.GetList<string>("multifield");
            Assert.IsTrue(list.Count() == 1);
            Assert.IsTrue(list.Except(new[] { "3" }).Count() == 0);
        }


        [TestMethod]
        public async Task AtomicCountersTest()
        {
            var obj = new APObject("object");
            obj.Set<int>("intfield", 0);
            await obj.SaveAsync();
            await obj.IncrementAsync("intfield", 10);
            await obj.DecrementAsync("intfield", 5);
            Assert.AreEqual(5, obj.Get<int>("intfield"));
        }

        [TestMethod]
        public async Task AtomicCountersWithoutPreInitializationTest()
        {
            var obj = new APObject("object");
            await obj.SaveAsync();
            await obj.IncrementAsync("intfield", 10);
            await obj.DecrementAsync("intfield", 5);
            Assert.AreEqual(5, obj.Get<int>("intfield"));
        }

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
            Assert.IsTrue(saved.CreatedAt.Subtract(DateTime.Now).Duration().Seconds < 60);
            Assert.IsTrue(saved.LastUpdatedAt.Subtract(DateTime.Now).Duration().Seconds < 60);
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
            Assert.IsTrue(Math.Abs(decimalField - pi) < 0.0001m );
            Assert.IsTrue(copy.Type == "object");
            Assert.IsTrue(copy.Revision == 1);
            Assert.IsTrue(copy.CreatedAt.Subtract(DateTime.Now).Duration().Seconds < 60);
            Assert.IsTrue(copy.LastUpdatedAt.Subtract(DateTime.Now).Duration().Seconds < 60);

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
        public async Task FindObjectByDataTimeField()
        {
            var time = DateTime.Now;
            var obj = ObjectHelper.NewInstance();
            obj.Set("datetimefield", time);
            await obj.SaveAsync();
            #if (NET40)
            await TaskEx.Delay(1000);
            #else
            await Task.Delay(1000);
            #endif
            var results = await APObjects.FindAllAsync("object", Query.Property("datetimefield").IsGreaterThanEqualTo(time));
            Assert.IsTrue(results.Count > 0);
            Assert.IsTrue(results.Exists(x => x.Id == obj.Id));

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
                catch (AppacitiveApiException)
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
            catch (AppacitiveApiException)
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
            Assert.IsTrue( Math.Abs(decimalField - pi) < 0.0001m);
            

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

            // Delay for index propagation on test bench.
            await Utilities.Delay(1500);
            var objects = await APObjects.FindAllAsync("object", query);
            Assert.IsNotNull(objects);
            Assert.IsTrue(objects.Count == 1);
            Console.WriteLine("page:{0} pageSize:{1} total: {2}", objects.PageNumber, objects.PageSize, objects.TotalRecords);

        }

        [TestMethod]
        public async Task FindAllObjectsAsyncWithInNestedQueryTest()
        {
            // Create the object
            dynamic apObject = new APObject("object");
            apObject.stringfield = Unique.String;
            apObject.intfield = 10;
            dynamic obj = await ObjectHelper.CreateNewAsync(apObject as APObject);

            dynamic apObject2 = new APObject("object");
            apObject2.stringfield = Unique.String;
            apObject2.intfield = 20;
            dynamic obj2 = await ObjectHelper.CreateNewAsync(apObject2 as APObject);

            // Search
            string string1ToSearch = obj.stringfield;
            string string2ToSearch = obj2.stringfield;
            var query = Query.And(new[] 
                        {
                            Query.Property("stringfield").IsIn(new List<string>{string1ToSearch, string2ToSearch}),
                            Query.Property("intfield").IsEqualTo(10)
                        });

            // Delay for index propagation on test bench.
            await Utilities.Delay(1500);
            var objects = await APObjects.FindAllAsync("object", query);
            Assert.IsNotNull(objects);
            Assert.IsTrue(objects.Count == 1);
            Console.WriteLine("page:{0} pageSize:{1} total: {2}", objects.PageNumber, objects.PageSize, objects.TotalRecords);

        }

        [TestMethod]
        public async Task FindNonExistantPageTest()
        {
            // Search
            var objects = await APObjects.FindAllAsync("object", Query.None, null, 10000, 500);
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
            var objects = await APObjects.FindAllAsync("object", Query.None, null, 1, 100);
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

            // Delay for index propagation on test bench.
            await Utilities.Delay(1500);

            // Search for the object with tags.
            var matches = await APObjects.FindAllAsync("object", Query.Tags.MatchAll(tags));
            Assert.IsTrue(matches != null);
            Assert.IsTrue(matches.Count == 1);
            Assert.IsTrue(matches[0] != null);
            Assert.IsTrue(matches[0].Id == obj.Id);
        }


        [TestMethod]
        public async Task FreetextAndQueryWithTagsMatchOneOrMoreTest()
        {
            var tag1 = Unique.String;
            var tag2 = Unique.String;
            var field1 = Unique.String;
            var field2 = Unique.String;
            // Create the test object 1.
            APObject obj1 = new APObject("object");
            obj1.Set<string>("stringfield", field1);
            obj1.AddTag(tag1);
            await obj1.SaveAsync();

            APObject obj2 = new APObject("object");
            obj2.Set<string>("stringfield", field2);
            obj2.AddTag(tag2);
            await obj2.SaveAsync();

            // Search for the object with tags and freetext as field1.
            var matches = await APObjects.FindAllAsync("object", field1, Query.Tags.MatchOneOrMore(tag1, tag2));
            Assert.IsTrue(matches != null);
            Assert.IsTrue(matches.Count == 1);
            Assert.IsTrue(matches[0] != null);
            Assert.IsTrue(matches[0].Id == obj1.Id);

            // Search for the object with tag1 and freetext as field2.
            matches = await APObjects.FindAllAsync("object", field2, Query.Tags.MatchOneOrMore(tag1));
            Assert.IsTrue(matches == null || matches.Count == 0);

            // Search for the object with tag2 and freetext as field2.
            matches = await APObjects.FindAllAsync("object", field2, Query.Tags.MatchOneOrMore(tag2));
            Assert.IsTrue(matches != null);
            Assert.IsTrue(matches.Count == 1);
            Assert.IsTrue(matches[0] != null);
            Assert.IsTrue(matches[0].Id == obj2.Id);
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
            Assert.IsTrue(matches[0] != null && matches[1] != null);
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
            
            // Delay for index propagation on test bench.
            await Utilities.Delay(1500);

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

            #if NET40
            await TaskEx.WhenAll(obj1.SaveAsync(), obj2.SaveAsync(), obj3.SaveAsync());
            #else
            await Task.WhenAll(obj1.SaveAsync(), obj2.SaveAsync(), obj3.SaveAsync());
            #endif
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

            #if NET40
            await TaskEx.WhenAll(obj1.SaveAsync(), obj2.SaveAsync());
            #else
            await Task.WhenAll(obj1.SaveAsync(), obj2.SaveAsync());
            #endif

            // Delay for index propagation on test bench.
            await Utilities.Delay(1500); 
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

            #if NET40
            await TaskEx.WhenAll(obj1.SaveAsync(), obj2.SaveAsync());
            #else
            await Task.WhenAll(obj1.SaveAsync(), obj2.SaveAsync());
            #endif

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
            #if NET40
            await TaskEx.WhenAll(tasks);
            #else
            await Task.WhenAll(tasks);
            #endif

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
            try
            {
                await obj.SaveAsync(obj.Revision - 1);
                Assert.Fail("No fault was raised on a bad revision update.");
            }
            catch (UpdateConflictException)
            {
            }   
        }

    }

    public class Score : APObject
    {
        public Score() : base("score")
        {
        }

        public Score(APObject obj)
            : base(obj)
        {
        }

        public int Points
        {
            get { return this.Get<int>("points", 0); }
            set { this.Set("points", value); }
        }

        public MultiValueCollection<string> Badges
        {
            get { return new MultiValueCollection<string>(this, "badges"); }
        }
    }
}
