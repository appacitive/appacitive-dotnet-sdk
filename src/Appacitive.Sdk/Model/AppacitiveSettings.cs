using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            this.UseHttps = true;
            this.HostName = string.Empty;
            this.PushSettings = new PushSettings();
        }

        public string HostName { get; set; }

        public bool UseHttps { get; set; } 

        public IDependencyContainer Factory { get; internal set; }

        public PushSettings PushSettings { get; private set; }

        private static IDependencyContainer GetDefaultContainer()
        {
            return InProcContainer.Instance;
        }
    }

    public class PushSettings
    {
        public PushSettings()
        {
            this.WhitelistedDomains = new List<Uri>();
        }

        public List<Uri> WhitelistedDomains { get; private set; }
    }
}
