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
    public class ObjectServiceFixture
    {

        [TestMethod]
        public async Task CreateObjectAsyncTest()
        {
            // Create obj
            var now = DateTime.Now;
            dynamic obj = ObjectHelper.NewInstance();

            CreateObjectResponse response = null;
            

            response = await (new CreateObjectRequest()
            {
                Object = obj,
                Environment = TestConfiguration.Environment
            }).ExecuteAsync();
            
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Status);
            Assert.IsTrue(response.Status.IsSuccessful);
            Assert.IsNotNull(response.Object);
            Console.WriteLine("Created obj id {0}", response.Object.Id);
            Console.WriteLine("Time taken {0} seconds", response.TimeTaken);
        }

        [TestMethod]
        public async Task GetObjectAsyncTest()
        {
            // Create obj
            var now = DateTime.Now;
            var apObject = await ObjectHelper.CreateNewAsync();


            // Get the apObject
            GetObjectResponse getResponse = null;
            getResponse = await (
                new GetObjectRequest()
                {   
                    Id = apObject.Id,
                    Type = apObject.Type
                }).ExecuteAsync();
            Assert.IsNotNull(getResponse);
            Assert.IsNotNull(getResponse.Status);
            Assert.IsTrue(getResponse.Status.IsSuccessful);
            Assert.IsNotNull(getResponse.Object);
            Console.WriteLine("Successfully read apObject id {0}", getResponse.Object.Id);
            Console.WriteLine("Time taken: {0} seconds", getResponse.TimeTaken);

        }

        [TestMethod]
        public async Task DeleteObjectAsyncTest()
        {
            // Create apObject
            var now = DateTime.Now;
            dynamic obj = new APObject("object");
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

            CreateObjectResponse response = null;
            response = await (new CreateObjectRequest()
                {
                    Object = obj
                }).ExecuteAsync();
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Status);
            Assert.IsTrue(response.Status.IsSuccessful);
            Assert.IsNotNull(response.Object);
            Console.WriteLine("Created apObject id {0}", response.Object.Id);
            Console.WriteLine("Time taken: {0} seconds", response.TimeTaken);

            // Delete the apObject
            var deleteObjectResponse = await (new DeleteObjectRequest()
                {
                    Id = response.Object.Id,
                    Type = response.Object.Type
                }).ExecuteAsync();
            Assert.IsNotNull(deleteObjectResponse, "Delete apObjectr response is null.");
            Assert.IsTrue(deleteObjectResponse.Status.IsSuccessful == true,
                          deleteObjectResponse.Status.Message ?? "Delete apObject operation failed.");

            // Try get the deleted apObject
            var getObjectResponse = await (
                new GetObjectRequest()
                    {
                        Id = response.Object.Id,
                        Type = response.Object.Type
                    }).ExecuteAsync();
            Assert.IsNotNull(getObjectResponse, "Get apObject response is null.");
            Assert.IsNull(getObjectResponse.Object, "Should not be able to get a deleted apObject.");
            Assert.IsTrue(getObjectResponse.Status.Code == "404", "Error code expected was not 404.");

        }

        [TestMethod]
        public async Task UpdateObjectAsyncTest()
        {
            // Create an apObject
            var now = DateTime.Now;
            dynamic obj = new APObject("object");
            obj.intfield = 1;
            obj.decimalfield = 10.0m;
            obj.datefield = "2012-12-20";
            obj.stringfield = "initial";
            obj.Tags.Add("initial");

            var createdResponse = await (new CreateObjectRequest()
                {
                    Object = obj
                }).ExecuteAsync();
            Assert.IsNotNull(createdResponse, "Object creation failed.");
            Assert.IsNotNull(createdResponse.Status, "Status is null.");
            Assert.IsTrue(createdResponse.Status.IsSuccessful,
                          createdResponse.Status.Message ?? "Create apObject failed.");
            var created = createdResponse.Object;

            // Update the apObject
            var updateRequest = new UpdateObjectRequest()
                {
                    Id = created.Id,
                    Type = created.Type
                };
            updateRequest.PropertyUpdates["intfield"] = "2";
            updateRequest.PropertyUpdates["decimalfield"] = 20.0m.ToString();
            updateRequest.PropertyUpdates["stringfield"] = null;
            updateRequest.PropertyUpdates["datefield"] = "2013-11-20";
            updateRequest.AddedTags.AddRange(new[] {"tag1", "tag2"});
            updateRequest.RemovedTags.AddRange(new[] {"initial"});
            var updatedResponse = await updateRequest.ExecuteAsync();


            Assert.IsNotNull(updatedResponse, "Update apObject response is null.");
            Assert.IsNotNull(updatedResponse.Status, "Update apObject response status is null.");
            Assert.IsNotNull(updatedResponse.Object, "Updated apObject is null.");
            var updated = updatedResponse.Object;
            Assert.IsTrue(updated.Get<string>("intfield") == "2");
            Assert.IsTrue(updated.Get<string>("decimalfield") == "20.0");
            Assert.IsTrue(updated.Get<string>("stringfield") == null);
            Assert.IsTrue(updated.Get<string>("datefield") == "2013-11-20");
            Assert.IsTrue(updated.Tags.Count() == 2);
            Assert.IsTrue(updated.Tags.Intersect(new[] {"tag1", "tag2"}).Count() == 2);

        }

        [TestMethod]
        public async Task BugId14Test()
        {
            // Ref: https://github.com/appacitive/Gossamer/issues/14
            // Updating a null property with a null value fails 
            // Create an apObject
            var now = DateTime.Now;
            dynamic obj = new APObject("object");
            obj.intfield = 1;
            obj.decimalfield = 10.0m;
            obj.stringfield = null;

            var createdResponse = await (new CreateObjectRequest()
            {
                Object = obj
            }).ExecuteAsync();
            Assert.IsNotNull(createdResponse, "Object creation failed.");
            Assert.IsNotNull(createdResponse.Status, "Status is null.");
            Assert.IsTrue(createdResponse.Status.IsSuccessful, createdResponse.Status.Message ?? "Create apObject failed.");
            var created = createdResponse.Object;

            // Update the apObject twice
            for (int i = 0; i < 2; i++)
            {
                var updateRequest = new UpdateObjectRequest()
                {
                    Id = created.Id,
                    Type = created.Type
                };
                updateRequest.PropertyUpdates["stringfield"] = null;
                var updatedResponse = await updateRequest.ExecuteAsync();

                Assert.IsNotNull(updatedResponse, "Update apObject response is null.");
                Assert.IsNotNull(updatedResponse.Status, "Update apObject response status is null.");
                Assert.IsTrue(updatedResponse.Status.IsSuccessful, updatedResponse.Status.Message ?? "NULL");
                Assert.IsNotNull(updatedResponse.Object, "Updated apObject is null.");
                var updated = updatedResponse.Object;
                Assert.IsTrue(updated["stringfield"] is NullValue);   
            }
        }

        [TestMethod]
        public async Task FindAllObjectAsyncFixture()
        {
            // Create atleast one apObject
            var now = DateTime.Now;
            dynamic obj = new APObject("object");
            obj.intfield = 66666;
            obj.decimalfield = 98765.0m;
            obj.datefield = "2012-12-20";
            obj.datetimefield = now.ToString("o");
            obj.stringfield = "Frank";
            obj.textfield = "Frand Sinatra was an amazing singer.";
            obj.boolfield = false;
            obj.geofield = "11.5,12.5";
            obj.listfield = "a";


            CreateObjectResponse response = null;
            response = await (new CreateObjectRequest()
            {
                Object = obj
            }).ExecuteAsync();

            ApiHelper.EnsureValidResponse(response);

            // Find all objects
            var findRequest = new FindAllObjectsRequest() { Type = "object" };
            var findResponse = await findRequest.ExecuteAsync();
            ApiHelper.EnsureValidResponse(findResponse);
            findResponse.Objects.ForEach(x => Console.WriteLine("Found apObject id {0}.", x.Id));
            Assert.IsNotNull(findResponse.PagingInfo);
            Assert.IsTrue(findResponse.PagingInfo.PageNumber == 1);
            Assert.IsTrue(findResponse.PagingInfo.TotalRecords > 0);
            Console.WriteLine("Paging info => pageNumber: {0}, pageSize: {1}, totalRecords: {2}",
                findResponse.PagingInfo.PageNumber,
                findResponse.PagingInfo.PageSize,
                findResponse.PagingInfo.TotalRecords);
        }

    }
}
