using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class RealTimeChannelFactory : IRealTimeChannelFactory
    {
        private static IRealTimeChannel _instance = ObjectFactory.Build<IRealTimeChannel>();
        
        public IRealTimeChannel CreateChannel()
        {
            return _instance;
        }
    }
}
