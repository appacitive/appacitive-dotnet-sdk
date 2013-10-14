using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class AppacitiveSettings
    {
        internal static readonly AppacitiveSettings Default = new AppacitiveSettings();

        public AppacitiveSettings()
        {
            this.Factory = GetDefaultContainer();
            this.UseApiSession = false;
            this.EnableRealTimeSupport = false;
        }

        public IDependencyContainer Factory { get; set; }

        public bool UseApiSession { get; set; }

        public bool EnableRealTimeSupport { get; set; }

        private static IDependencyContainer GetDefaultContainer()
        {
            return InProcContainer.Instance;
        }
    }
}
