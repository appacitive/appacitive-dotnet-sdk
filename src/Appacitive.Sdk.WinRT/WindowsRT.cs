using Appacitive.Sdk.Aspnet;
using Appacitive.Sdk.Interfaces;
using Appacitive.Sdk.Wcf;
using Appacitive.Sdk.WinRT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static class WindowsRT
    {
        public static readonly IApplicationHost Host = WindowsHost.Instance;
        public static readonly IApplicationHost Aspnet = AspnetApplicationHost.Instance;
        public static readonly IApplicationHost Wcf = WcfApplicationHost.Instance;
    }
}
