using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Internal;



namespace Appacitive.Sdk.Services
{
    public class SendPushNotificationRequest : PostOperation<SendPushNotificationResponse>
    {
        public PushNotification Push { get; set; }

        public override byte[] ToBytes()
        {
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Serialize(this.Push);
        }

        protected override string GetUrl()
        {
            return Urls.For.SendPushNotification(this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
