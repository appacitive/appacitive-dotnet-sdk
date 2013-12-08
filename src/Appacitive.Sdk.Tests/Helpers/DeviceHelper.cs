﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    internal static class DeviceHelper
    {
        public static async Task<APDevice> CreateNewAsync(APDevice device = null)
        {
            Console.WriteLine("Creating new device");
            if (device == null)
            {
                device = new APDevice(DeviceType.iOS)
                {
                    DeviceToken = Guid.NewGuid().ToString(),
                    Badge = 1,
                    Location = new Geocode(10, 10),
                    TimeZone = Timezone.Create(5, 30)
                };
            }
            await device.SaveAsync();
            Console.WriteLine("Created new device with id {0}.", device.Id);
            return device;
        }

        public static APDevice NewDevice()
        {
            return new APDevice(DeviceType.iOS)
                {
                    DeviceToken = Guid.NewGuid().ToString(),
                    Badge = 1,
                    Location = new Geocode(10, 10),
                    TimeZone = Timezone.Create(5, 30)
                };
        }
    }
}
