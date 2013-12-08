using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class DeviceFixture
    {
        [TestMethod]
        public async Task RegisterDeviceAsyncTest()
        {
            APDevice device = new APDevice(DeviceType.iOS)
            {
                DeviceToken = Guid.NewGuid().ToString(),
                Badge = 1,
                Location = new Geocode(10,10),
                TimeZone  = Timezone.Create(5,30),
            };
            device.Channels.Add("a");
            device.Channels.Add("b");
            await device.SaveAsync();
            Console.WriteLine("Created new device with id {0}.", device.Id);
        }

        [TestMethod]
        public async Task GetDeviceAsyncTest()
        {
            // Create a new device
            var created = await DeviceHelper.CreateNewAsync();
            created.Channels.AddRange(new[] { "x", "y", "z" });
            var count = created.Channels.Count;
            await created.SaveAsync();
            var device = await APDevices.GetAsync(created.Id);
            Assert.IsTrue(device.Channels.Count == count);
            Assert.IsNotNull(device);
            Assert.IsTrue(device.Id == created.Id);
            Assert.IsTrue(device.DeviceToken == created.DeviceToken);
            Assert.IsTrue(device.DeviceType == created.DeviceType);
            Assert.IsTrue(device.Location.ToString() == created.Location.ToString());
            Assert.IsTrue(device.TimeZone.Equals(created.TimeZone));
            
        }

        [TestMethod]
        public async Task DeleteDeviceAsyncTest()
        {
            var created = await DeviceHelper.CreateNewAsync();
            await APDevices.DeleteAsync(created.Id);
            // Try to get it.
            try
            {
                var shouldNotExist = await APDevices.GetAsync(created.Id);
                Assert.Fail("Able to retrieve deleted apObject.");
            }
            catch (Appacitive.Sdk.Net45.AppacitiveException)
            {   
            }
        }

        [TestMethod]
        public async Task UpdateDeviceAsyncTest()
        {
            var created = await DeviceHelper.CreateNewAsync();
            created.DeviceToken = Guid.NewGuid().ToString();
            created.Badge = created.Badge + 2;
            created.Location = new Geocode(20, 20);
            created.TimeZone = Timezone.Create(10, 30);
            created.IsActive = false;
            await created.SaveAsync();

            var updated = await APDevices.GetAsync(created.Id);
            Assert.IsTrue(updated != null);
            Assert.IsTrue(updated.Id == created.Id);
            Assert.IsTrue(updated.DeviceType == created.DeviceType);
            Assert.IsTrue(updated.DeviceToken == created.DeviceToken);
            Assert.IsTrue(updated.Badge == created.Badge);
            Assert.IsTrue(updated.IsActive == created.IsActive);
            Assert.IsTrue(updated.TimeZone.Equals(created.TimeZone));
            Assert.IsTrue(updated.Location.Equals(created.Location));
        }

    }
}
