using Appacitive.Sdk.Realtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class PushFixture
    {
        [Ignore]
        [TestMethod]
        public async Task BroadcastPushAsyncTest()
        {
            string id = await PushNotification
                .Broadcast("Push from .NET SDK")
                .WithBadge("+1")
                .WithData(new { field1 = "value1", field2 = "value2" })
                .SendAsync();
            Console.WriteLine("Send push notification with id {0}.", id);
        }

        [Ignore]
        [TestMethod]
        public async Task QueryBasedPushAsyncTest()
        {
            string id = await PushNotification
                .ToQueryResult("Push to query",Query.Property("devicetype").IsEqualTo("ios").AsString() ) 
                .WithBadge("+1")
                .WithData(new { field1 = "value1", field2 = "value2" })
                .SendAsync();
            Console.WriteLine("Send push notification with id {0}.", id);
        }


        [TestMethod]
        public void SerializerToastMessageTest()
        {
            var msg = PushNotification
                .Broadcast("This is a test message.")
                .WithBadge("+1")
                .WithExpiry(100000)
                .WithPlatformOptions(new IOsOptions { SoundFile = "soundfile" })
                .WithPlatformOptions(new AndroidOptions { NotificationTitle = "title" })
                .WithPlatformOptions(
                    new WindowsPhoneOptions
                    {
                        Notification = new ToastNotification
                        {
                            Text1 = "text1", 
                            Text2 = "text2",
                            Path = "path"
                        }
                    });
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            Console.WriteLine(Encoding.UTF8.GetString(serializer.Serialize(msg))); 
        }

        [TestMethod]
        public void SerializerRawPushNotificationTest()
        {
            var msg = PushNotification
                .Broadcast("This is a test message.")
                .WithBadge("+1")
                .WithExpiry(100000)
                .WithPlatformOptions(new IOsOptions { SoundFile = "soundfile" })
                .WithPlatformOptions(new AndroidOptions { NotificationTitle = "title" })
                .WithPlatformOptions(
                    new WindowsPhoneOptions
                    {
                        Notification = new RawNotification
                        {
                            RawData = "raw string data.."
                        }
                    });
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            Console.WriteLine(Encoding.UTF8.GetString(serializer.Serialize(msg)));
        }

        [TestMethod]
        public void SerializerFlipTilePushNotificationTest()
        {
            var msg = PushNotification
                .Broadcast("This is a test message.")
                .WithBadge("+1")
                .WithExpiry(100000)
                .WithPlatformOptions(new IOsOptions { SoundFile = "soundfile" })
                .WithPlatformOptions(new AndroidOptions { NotificationTitle = "title" })
                .WithPlatformOptions(
                    new WindowsPhoneOptions
                    {
                        Notification = TileNotification.CreateNewFlipTile(
                            new FlipTile
                            {
                                BackBackgroundImage = "bbimage",
                                BackContent = "back content",
                                BackTitle = "back title",
                                FrontBackgroundImage = "fbi",
                                FrontCount = "front count",
                                FrontTitle = "front title",
                                SmallBackgroundImage = "sbi",
                                TileId = "tileid",
                                WideBackBackgroundImage = "wbi",
                                WideBackContent = "wbc",
                                WideBackgroundImage = "wbi"
                            })
                    });
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            Console.WriteLine(Encoding.UTF8.GetString(serializer.Serialize(msg)));
        }

        [TestMethod]
        public void SerializerIconicTilePushNotificationTest()
        {
            var msg = PushNotification
                .Broadcast("This is a test message.")
                .WithBadge("+1")
                .WithExpiry(100000)
                .WithPlatformOptions(new IOsOptions { SoundFile = "soundfile" })
                .WithPlatformOptions(new AndroidOptions { NotificationTitle = "title" })
                .WithPlatformOptions(
                    new WindowsPhoneOptions
                    {
                        Notification = TileNotification.CreateNewIconicTile(
                            new IconicTile
                            {
                                BackgroundColor = "bc",
                                WideContent1 = "wc1",
                                WideContent2 = "wc2",
                                FrontTitle = "front title"
                            },
                            new FlipTile
                            {
                                BackBackgroundImage = "bbimage",
                                BackContent = "back content",
                                BackTitle = "back title",
                                FrontBackgroundImage = "fbi",
                                FrontCount = "front count",
                                FrontTitle = "front title",
                                SmallBackgroundImage = "sbi",
                                TileId = "tileid",
                                WideBackBackgroundImage = "wbi",
                                WideBackContent = "wbc",
                                WideBackgroundImage = "wbi"
                            })
                    });
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            Console.WriteLine(Encoding.UTF8.GetString(serializer.Serialize(msg)));
        }


        [Ignore]
        [TestMethod]
        public async Task PushToDevicesAsyncTest()
        {
            string id = await PushNotification
                .ToDeviceIds("This is a device specific push.", "26430875972534723")
                .WithBadge("+1")
                .WithData(new { field1 = "value1", field2 = "value2" })
                .SendAsync();
            Console.WriteLine("Send push notification with id {0}.", id);
        }
    }
}
