using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static class Appacitive
    {
        public static void Setup(object settings)
        {
        }
    }


    internal static class AppacitiveContext
    {
        public static string ApiKey { get; set; }

        public static Environment Environment { get; set; }

        public static string SessionToken { get; set; }

        public static string UserToken { get; set; }

        public static Geocode UserLocation { get; set; }

        public static IObjectFactory ObjectFactory { get; set; }
    }

    public enum Environment
    {
        Sandbox,
        Live
    }

    public class Geocode
    {
        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }

    public enum Verbosity
    {
        Info,
        Verbose
    }
}
