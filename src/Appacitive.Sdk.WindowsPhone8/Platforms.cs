using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.WindowsPhone8
{
    public static class Platforms
    {
        public static readonly Platform WP8 = WP8Platform.Instance;

        public static void Run()
        {
            var settings = new AppacitiveSettings();
            settings.PushSettings.WhitelistedDomains.Add(new Uri("http://www.yahoo.com"));
            App.Initialize(Platforms.WP8, "1234", "", Environment.Live, settings);
            var channel = App.Current.CurrentDevice.GetChannel();
            channel.HttpNotificationReceived += channel_HttpNotificationReceived;
            channel.ShellToastNotificationReceived += channel_ShellToastNotificationReceived;
                
        }

        static void channel_ShellToastNotificationReceived(object sender, Microsoft.Phone.Notification.NotificationEventArgs e)
        {
            throw new NotImplementedException();
        }

        static void channel_HttpNotificationReceived(object sender, Microsoft.Phone.Notification.HttpNotificationEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
