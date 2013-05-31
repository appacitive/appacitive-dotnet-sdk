using Appacitive.Net45;
using Appacitive.Sdk;
using Appacitive.Sdk.Realtime;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            App.Initialize(WindowsRT.Host, "111", Appacitive.Sdk.Environment.Sandbox, new AppacitiveSettings
            {
                EnableRealTimeSupport = true
            });
            var t = Run();
            try
            {
                t.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadKey(true);
        }

        static async Task Run()
        {
            Console.Write("Enter username: ");
            var username = Console.ReadLine();
            Console.Write("Enter password: ");
            var password = Console.ReadLine();

            var session = await App.LoginAsync(new UsernamePasswordCredentials(username, password));
            Console.WriteLine("User id {0} logged in.", session.LoggedInUser.Id);
            Messaging.Hubs.Joined += m =>
                {
                    var msg = m as JoinedHubMessage;
                    Console.WriteLine("User id {0} join the chatroom.", msg.User);
                    Console.Write(">");
                };
            Messaging.Hubs.Left += m =>
            {
                var msg = m as JoinedHubMessage;
                Console.WriteLine("User id {0} left the chatroom.", msg.User);
                Console.Write(">");
            };
            Messaging.Inbox.NewMessage += m =>
                {
                    var msg = m as NewNotificationMessage;
                    if (msg == null) return;
                    Console.WriteLine(System.Environment.NewLine + "User id {0} says - '{1}'", msg.Sender, ParseMessage(msg.Payload));
                    Console.Write(">");
                };

            await Messaging.JoinHubAsync("my_chat_room");
            string message = null;
            do
            {
                Console.Write(">");
                message = Console.ReadLine();
                if (message == "exit")
                {
                    await Messaging.LeaveHubAsync("my_chat_room");
                    return;
                }
                await Messaging.SendMessageAsync(new { text = message }, "my_chat_room");
            } while (true);
        }

        private static string ParseMessage(IJsonObject obj)
        {
            var json = JObject.Parse(obj.AsString());
            return json["text"].ToString();
        }
    }
}
