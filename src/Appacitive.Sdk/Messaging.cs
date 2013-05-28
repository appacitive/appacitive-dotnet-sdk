using Appacitive.Sdk.Realtime;
using Appacitive.Sdk.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Realtime
{
    public static class Messaging
    {
        public static readonly MessageSubscriber Inbox = new MessageSubscriber();

        public static readonly HubSubscriber Hubs = new HubSubscriber();

        public async static Task SendMessageAsync(object message, params string[] userIds)
        {
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            var msg = new SendToUsers
            {
                Users = userIds,
                Payload = JObject.FromObject(message)
            };
            await App.SendMessageAsync(msg);
        }

        public async static Task SendMessageAsync(object message, string hub)
        {
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            var msg = new SendToHub
            {
                Hub = hub,
                Payload = JObject.FromObject(message)
            };
            await App.SendMessageAsync(msg);
        }

        public async static Task JoinHubAsync(string hub)
        {
            var msg = new SubscribeToHubMessage { Hub = hub };
            await App.SendMessageAsync(msg);
        }

        public async static Task LeaveHubAsync(string hub)
        {
            var msg = new UnsubscribeFromHubMessage { Hub = hub };
            await App.SendMessageAsync(msg);
        }
    }
}
