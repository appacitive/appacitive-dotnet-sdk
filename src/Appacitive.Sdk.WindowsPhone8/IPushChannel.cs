using System;
using Appacitive.Sdk.Internal;
using Microsoft.Phone.Notification;
#if WINDOWS_PHONE
using Appacitive.Sdk.WindowsPhone8;
#elif WINDOWS_PHONE7
using Appacitive.Sdk.WindowsPhone7;
#endif

namespace Appacitive.Sdk
{
    public interface IPushChannel
    {
        event EventHandler<HttpNotificationEventArgs> HttpNotificationReceived;
        event EventHandler<NotificationEventArgs> ShellToastNotificationReceived;
    }
}
