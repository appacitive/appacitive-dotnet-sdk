using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class MultiCallerFixture
    {
        [TestMethod]
        public async Task AllMultiCallerOperationsTest()
        {
            var newObj = ObjectHelper.NewInstance();
            var existing = await ObjectHelper.CreateNewAsync();
            var objToBeDeleted = await ObjectHelper.CreateNewAsync();

            var newConn = APConnection.New("sibling")
                            .FromNewObject("object", ObjectHelper.NewInstance())
                            .ToNewObject("object", ObjectHelper.NewInstance());
            var existingConn = await APConnection.New("sibling")
                            .FromNewObject("object", ObjectHelper.NewInstance())
                            .ToNewObject("object", ObjectHelper.NewInstance())
                            .SaveAsync();
            var toBeDeletedConn = await APConnection.New("sibling")
                            .FromNewObject("object", ObjectHelper.NewInstance())
                            .ToNewObject("object", ObjectHelper.NewInstance())
                            .SaveAsync();

            existing.Set("textfield", Guid.NewGuid().ToString());
            existingConn.SetAttribute("testAttr", Guid.NewGuid().ToString());
            APBatch batch = new APBatch();
            batch
                .AddObjectsToCreate(newObj)
                .AddObjectToUpdate(existing.Id, existing)
                .AddConnectionsToCreate(newConn)
                .AddConnectionToUpdate(existingConn.Id, existingConn)
                .AddObjectToDelete("object", objToBeDeleted.Id)
                .AddConnectionToDelete(toBeDeletedConn.Type, toBeDeletedConn.Id);
            await batch.ExecuteAsync();

            Assert.AreEqual(1, batch.CreatedObjects.Count());
            Assert.AreEqual(1, batch.CreatedConnections.Count());
            Assert.AreEqual(1, batch.UpdatedObjects.Count());
            Assert.AreEqual(1, batch.UpdatedConnections.Count());
            Assert.AreEqual(1, batch.DeletedObjects.Count());
            Assert.AreEqual(1, batch.DeletedConnections.Count());

        }


        [TestMethod]
        public async Task MulticallerCanOnlyBeExecutedOnceTest()
        {
            var newObj = ObjectHelper.NewInstance();
            APBatch batch = new APBatch().AddObjectsToCreate(newObj);
            await batch.ExecuteAsync();
            try
            {
                await batch.ExecuteAsync();
                Assert.Fail("This should have failed.");
            }
            catch (AppacitiveRuntimeException)
            {
            }
        }

        [TestMethod]
        public async Task MultiCallerWithUserUpdateTest()
        {
            var user = await UserHelper.CreateNewUserAsync();
            user.SetAttribute("IsTestUser", "true");
            APBatch batch = new APBatch().AddObjectToUpdate(user.Id, user);
            await batch.ExecuteAsync();
            Assert.AreEqual(1, batch.UpdatedObjects.Count());
        }
    }


}
