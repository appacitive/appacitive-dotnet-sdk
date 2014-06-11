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
            var conn = APConnection.New("link").FromNewObject("parent", parent).ToNewObject("child", child);
            await conn.SaveAsync();

            var parent2 = await conn.Endpoints["parent"].GetObjectAsync();
            var child2 = await conn.Endpoints["child"].GetObjectAsync();
            Assert.IsTrue(parent2 != null && child2 != null);
            Assert.IsTrue(parent2.Get<string>("stringfield") == "parent");
            Assert.IsTrue(child2.Get<string>("stringfield") == "child");

            // Swap and test
            parent = ObjectHelper.NewInstance();
            parent["stringfield"] = "parent";
            child = ObjectHelper.NewInstance();
            child["stringfield"] = "child";
            conn = APConnection.New("link").FromNewObject("child", child).ToNewObject("parent", parent);
            await conn.SaveAsync();

            parent2 = await conn.Endpoints["parent"].GetObjectAsync();
            child2 = await conn.Endpoints["child"].GetObjectAsync();
            Assert.IsTrue(parent2 != null && child2 != null);
            Assert.IsTrue(parent2.Get<string>("stringfield") == "parent");
            Assert.IsTrue(child2.Get<string>("stringfield") == "child");
        }
        
        [TestMethod]
        public async Task GetEndpointContentTest()
        {
            var existing = await ObjectHelper.CreateNewAsync();
            var newObj = ObjectHelper.NewInstance();
            var conn = APConnection
                .New("link")
                .FromExistingObject("parent", existing.Id)
                .ToNewObject("child", newObj);
            await conn.SaveAsync();

            Assert.IsTrue(conn.GetEndpointId("parent") == existing.Id);
            Assert.IsTrue( string.IsNullOrWhiteSpace(conn.GetEndpointId("child")) == false );
            
            // Get endpoints
            var child = await conn.GetEndpointObjectAsync("child");
            var parent = await conn.GetEndpointObjectAsync("parent");
            Assert.IsNotNull(child);
            Assert.IsNotNull(parent);
            Assert.IsTrue(parent.Id == existing.Id);
        }

        [TestMethod]
        public async Task CreateConnectionWithNewUserAndNewDevice()
        {
            var device = DeviceHelper.NewDevice();
            var user = UserHelper.NewUser();
            var conn = APConnection.New("my_device")
                .FromNewObject("device", device)
                .ToNewObject("user", user);
            await conn.SaveAsync();

            Assert.IsTrue(string.IsNullOrWhiteSpace(conn.Id) == false);
            Console.WriteLine("Created connection with id: {0}", conn.Id);
        }

        [TestMethod]
        public async Task GetConnectedObjectsOnConnectionWithSameTypeAndDifferentLabels()
        {
            var parent = await ObjectHelper.CreateNewAsync();
            var child1 = await ObjectHelper.CreateNewAsync();
            var child2 = await ObjectHelper.CreateNewAsync();
            // Create connections
            await APConnection.New("link")
                .FromExistingObject("parent", parent.Id)
                .ToExistingObject("child", child1.Id)
                .SaveAsync();

            await APConnection.New("link")
                .FromExistingObject("parent", parent.Id)
                .ToExistingObject("child", child2.Id)
                .SaveAsync();

            // Get connected objects
            var objects = await parent.GetConnectedObjectsAsync("link", label: "child");
            Assert.IsTrue(objects.Count == 2);
            Assert.IsTrue(objects.Select(a => a.Id).Intersect(new[] { child1.Id, child2.Id }).Count() == 2);
        }



        [TestMethod]
        public async Task GetConnectionsOnConnectionWithSameTypeAndDifferentLabels()
        {
            var parent = await ObjectHelper.CreateNewAsync();
            var child1 = await ObjectHelper.CreateNewAsync();
            var child2 = await ObjectHelper.CreateNewAsync();
            // Create connections
            var conn1 = APConnection.New("link")
                .FromExistingObject("parent", parent.Id)
                .ToExistingObject("child", child1.Id);
            await conn1.SaveAsync();

            var conn2 = APConnection.New("link")
                .FromExistingObject("parent", parent.Id)
                .ToExistingObject("child", child2.Id);
            await conn2.SaveAsync();

            // Get connected objects
            var connections = await parent.GetConnectionsAsync("link", label: "child");
            Assert.IsTrue(connections.Count == 2);
            Assert.IsTrue(connections.Select(a => a.Id).Intersect(new[] { conn1.Id, conn2.Id }).Count() == 2);
        }


        [TestMethod]
        public async Task GetConnectedObjectsForSameTypeAndDifferentLabelsWithoutLabelTest()
        {
            var parent = await ObjectHelper.CreateNewAsync();
            var child1 = await ObjectHelper.CreateNewAsync();
            var child2 = await ObjectHelper.CreateNewAsync();
            // Create connections
            await APConnection.New("link")
                .FromExistingObject("parent", parent.Id)
                .ToExistingObject("child", child1.Id)
                .SaveAsync();

            await APConnection.New("link")
                .FromExistingObject("parent", parent.Id)
                .ToExistingObject("child", child2.Id)
                .SaveAsync();

            // Get connected objects
            try
            {
                var objects = await parent.GetConnectedObjectsAsync("link");
                Assert.Fail("This call should have failed since we did not specify the label to retreive.");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message == "Label is required for edge 'link'.");
            }
        }

        [TestMethod]
        public async Task CreateDuplicateConnectionAsyncTest()
        {

            var o1 = new APObject("object");
            var o2 = new APObject("object");
            var conn = await APConnection
                            .New("sibling")
                            .FromNewObject("object", o1)
                            .ToNewObject("object", o2)
                            .SaveAsync();

            var dupe = await APConnection
                            .New("sibling")
                            .FromExistingObject("object", o1.Id)
                            .ToExistingObject("object", o2.Id)
                            .SaveAsync();
            Assert.AreEqual(conn.Id, dupe.Id);
        }

        [TestMethod]
        public async Task CreateDuplicateConnectionWithFaultAsyncTest()
        {

            var o1 = new APObject("object");
            var o2 = new APObject("object");
            var conn = await APConnection
                            .New("sibling")
                            .FromNewObject("object", o1)
                            .ToNewObject("object", o2)
                            .SaveAsync();

            try
            {
                var dupe = await APConnection
                                .New("sibling")
                                .FromExistingObject("object", o1.Id)
                                .ToExistingObject("object", o2.Id)
                                .SaveAsync(throwIfAlreadyExists: true);
                Assert.Fail("Duplicate connection creation did not fault.");
            }
            catch (DuplicateObjectException)
            {   
            }
        }
        

        [TestMethod]
        public async Task CreateConnectionBetweenNewObjectsAsyncTest()
        {
            var obj1 = new APObject("object");
            var obj2 = new APObject("object");
            var conn = APConnection
                            .New("sibling")
                            .FromNewObject("object", obj1)
                            .ToNewObject("object", obj2);

            await conn.SaveAsync();
            Assert.IsTrue(string.IsNullOrWhiteSpace(conn.Id) == false);
            Console.WriteLine("Created connection with id: {0}", conn.Id);
            Assert.IsTrue(string.IsNullOrWhiteSpace(obj1.Id) == false);
            Console.WriteLine("Created new apObject with id: {0}", obj1.Id);
            Assert.IsTrue(string.IsNullOrWhiteSpace(obj2.Id) == false);
            Console.WriteLine("Created new apObject with id: {0}", obj2.Id);
        }


        [TestMethod]
        public async Task UpdateConnectionAsyncTest()
        {
            dynamic conn = APConnection
                .New("sibling")
                .FromNewObject("object", ObjectHelper.NewInstance())
                .ToNewObject("object", ObjectHelper.NewInstance());
            conn.field1 = "test";
            conn.field2 = 15L;
            await conn.SaveAsync();

            Console.WriteLine("Created connection with id: {0}", conn.Id);

            // Update the connection
            conn.field1 = "updated";
            conn.field2 = 11L;
            await conn.SaveAsync();

            // Get the connection
            APConnection read = await APConnections.GetAsync("sibling", conn.Id);
            // Asserts
            Assert.IsTrue( read.Get<string>("field1") == "updated");
            Assert.IsTrue( read.Get<int>("field2") == 11L);
        }

        [TestMethod]
        public async Task CreateConnectionBetweenNewAndExistingObjectsAsyncTest()
        {
            var obj1 = await ObjectHelper.CreateNewAsync();
            var obj2 = new APObject("object");
            var conn = APConnection
                            .New("sibling")
                            .FromExistingObject("object", obj1.Id)
                            .ToNewObject("object", obj2);
            await conn.SaveAsync();
            Assert.IsTrue(string.IsNullOrWhiteSpace(conn.Id) == false);
            Console.WriteLine("Created connection with id: {0}", conn.Id);
            Assert.IsTrue(string.IsNullOrWhiteSpace(obj2.Id) == false);
            Console.WriteLine("Created new apObject with id: {0}", obj1.Id);
            // Ensure that the endpoint ids match
            Assert.IsTrue(conn.Endpoints.ToArray().Select(x => x.ObjectId).Intersect(new[] { obj1.Id, obj2.Id }).Count() == 2);
        }


        [TestMethod]
        public async Task UpdatePartialConnectionAsyncTest()
        {
            // Create new connection
            var obj1 = await ObjectHelper.CreateNewAsync();
            var obj2 = new APObject("object");
            var conn = APConnection
                            .New("sibling")
                            .FromExistingObject("object", obj1.Id)
                            .ToNewObject("object", obj2);
            await conn.SaveAsync();

            // Update
            var conn2 = new APConnection("sibling", conn.Id);
            var value = Guid.NewGuid().ToString();
            conn["field1"] = value;
            await conn.SaveAsync();

            var updated = await APConnections.GetAsync("sibling", conn.Id);
            Assert.IsTrue(updated.Get<string>("field1") == value);
        }

        [TestMethod]
        public async Task CreateConnection2BetweenNewAndExistingObjectsAsyncTest()
        {
            var obj1 = await ObjectHelper.CreateNewAsync();
            var obj2 = new APObject("object");
            var conn = APConnection
                            .New("sibling")
                            .FromExistingObject("object", obj1.Id)
                            .ToNewObject("object", obj2);
            await conn.SaveAsync();
            Assert.IsTrue(string.IsNullOrWhiteSpace(conn.Id) == false);
            Console.WriteLine("Created connection with id: {0}", conn.Id);
            Assert.IsTrue(string.IsNullOrWhiteSpace(obj2.Id) == false);
            Console.WriteLine("Created new apObject with id: {0}", obj1.Id);
            // Ensure that the endpoint ids match
            Assert.IsTrue(conn.Endpoints.ToArray().Select(x => x.ObjectId).Intersect(new[] { obj1.Id, obj2.Id }).Count() == 2);
        }

        [TestMethod]
        public async Task CreateConnectionBetweenExistingObjectsAsyncTest()
        {
            var obj1 = await ObjectHelper.CreateNewAsync();
            var obj2 = await ObjectHelper.CreateNewAsync();
            var conn = APConnection
                            .New("sibling")
                            .FromExistingObject("object", obj1.Id)
                            .ToExistingObject("object", obj2.Id);
            await conn.SaveAsync();
            Assert.IsTrue(string.IsNullOrWhiteSpace(conn.Id) == false);
            Console.WriteLine("Created connection with id: {0}", conn.Id);
            Assert.IsTrue(string.IsNullOrWhiteSpace(obj2.Id) == false);
            Console.WriteLine("Created new apObject with id: {0}", obj1.Id);
            // Ensure that the endpoint ids match
            Assert.IsTrue(conn.Endpoints.ToArray().Select(x => x.ObjectId).Intersect(new[] { obj1.Id, obj2.Id }).Count() == 2);
        }

        [TestMethod]
        public async Task GetConnectionAsyncTest()
        {
            // Create a new connection
            var conn = await ConnectionHelper.CreateNew();
            var read = await APConnections.GetAsync(conn.Type, conn.Id);
            Assert.IsTrue(read != null);
            Assert.IsTrue(read.Id == conn.Id);
            Assert.IsTrue(conn.Endpoints.ToArray().Select(x => x.ObjectId).Intersect(read.Endpoints.ToArray().Select( x => x.ObjectId)).Count() == 2);
        }

        [TestMethod]
        public async Task GetConnectionByEndpointsAsyncTest()
        {
            // Create a new connection
            var conn = await ConnectionHelper.CreateNew();
            var endpoints = conn.Endpoints.ToArray();
            var read = await APConnections.GetAsync(conn.Type, endpoints[0].ObjectId, endpoints[1].ObjectId);
            Assert.IsTrue(read != null);
            Assert.IsTrue(read.Id == conn.Id);
            Assert.IsTrue(conn.Endpoints.ToArray().Select(x => x.ObjectId).Intersect(read.Endpoints.ToArray().Select(x => x.ObjectId)).Count() == 2);
        }

        [TestMethod]
        public async Task FindAllConnectionsAsyncTest()
        {
            // Create a new connection
            var conn = await ConnectionHelper.CreateNew();
            // Find all
            var connections = await APConnections.FindAllAsync("sibling");
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
            await APConnections.DeleteAsync(conn.Type, conn.Id);
            // Try and get the connection
            try
            {
                var read = await APConnections.GetAsync(conn.Type, conn.Id);
                Assert.Fail("No exception was raised on reading deleted connection.");
            }
            catch (ObjectNotFoundException)
            {
                
            }
        }

        [TestMethod]
        public async Task BulkDeleteConnectionTest()
        {
            // Create a new connection
            var conn1 = await ConnectionHelper.CreateNew();
            var conn2 = await ConnectionHelper.CreateNew();
            // Delete the connection
            await APConnections.MultiDeleteAsync(conn1.Type, new[] { conn1.Id, conn2.Id });
            // Try and get the connection
            try
            {
                var read = await APConnections.GetAsync(conn1.Type, conn1.Id);
                Assert.Fail("No exception was raised on reading deleted connection.");
            }
            catch (ObjectNotFoundException)
            {   
            }

            try
            {
                var read = await APConnections.GetAsync(conn2.Type, conn2.Id);
                Assert.Fail("No exception was raised on reading deleted connection.");
            }
            catch (ObjectNotFoundException)
            {   
            }
        }
    }
}
