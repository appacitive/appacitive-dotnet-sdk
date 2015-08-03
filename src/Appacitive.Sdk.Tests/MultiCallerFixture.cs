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
            batch.SaveObject(newObj);
            batch.SaveObject(existing);
            batch.SaveConnection(newConn);
            batch.SaveConnection(existingConn);
            batch.DeleteObject(objToBeDeleted);
            batch.DeleteConnection(toBeDeletedConn);
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
            APBatch batch = new APBatch();
            batch.SaveObject(newObj);
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
            APBatch batch = new APBatch();
            batch.SaveObject(user);
            await batch.ExecuteAsync();
            Assert.AreEqual(1, batch.UpdatedObjects.Count());
        }


        [TestMethod]
        public void HasBatchReferencesShouldBeTrueForConnectionsWithBatchReferences()
        {
            APBatch batch = new APBatch();
            var obj = ObjectHelper.NewInstance();
            var objReference = batch.SaveObject(obj);
            var connA = new APConnection("type", "labelA", "IdA", "labelB", "idB");
            var connB = new APConnection("type", "labelA", obj, "labelB", obj);
            var connC = new APConnection("type", "labelA", obj, "labelB", "IdB");
            var connD = new APConnection("type", "labelA", "IdA", "labelB", objReference);
            var connE = new APConnection("type", "labelA", objReference, "labelB", objReference);
            var connF = new APConnection("type", "labelA", obj, "labelB", objReference);
            
            Assert.IsFalse(connA.Endpoints.HasBatchReference);
            Assert.IsFalse(connB.Endpoints.HasBatchReference);
            Assert.IsFalse(connC.Endpoints.HasBatchReference);
            Assert.IsTrue(connD.Endpoints.HasBatchReference);
            Assert.IsTrue(connE.Endpoints.HasBatchReference);
            Assert.IsTrue(connF.Endpoints.HasBatchReference);
        }


        [TestMethod]
        public async Task SaveForConnectionWithBatchReferenceShouldFaultTest()
        {
            APBatch batch = new APBatch();
            var obj = ObjectHelper.NewInstance();
            var objReference = batch.SaveObject(obj);
            var conn = APConnection
                .New("siblings")
                .FromNewObject("object", ObjectHelper.NewInstance())
                .ToBatchObjectReference("object", objReference);
            try
            {
                await conn.SaveAsync();
                Assert.Fail("This should have failed.");
            }
            catch (AppacitiveRuntimeException)
            {
            }   
        }


        [TestMethod]
        public async Task MultiCallerWithInternalReferenceTest()
        {
            APBatch batch = new APBatch();
            var newObj = ObjectHelper.NewInstance();
            var objReference = batch.SaveObject(newObj);
            var newConn = APConnection.New("sibling")
                            .FromNewObject("object", ObjectHelper.NewInstance())
                            .ToBatchObjectReference("object", objReference);
            var connReference = batch.SaveConnection(newConn);
            
            await batch.ExecuteAsync();

            Assert.AreEqual(1, batch.CreatedObjects.Count());
            Assert.AreEqual(1, batch.CreatedConnections.Count());
            Assert.IsNotNull(batch.CreatedObjects[objReference]);
            Assert.IsNotNull(batch.CreatedConnections[connReference]);

        }
    }


}
