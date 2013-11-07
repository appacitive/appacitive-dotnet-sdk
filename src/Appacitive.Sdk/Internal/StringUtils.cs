using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    internal static class StringUtils
    {
        public static string EscapeAndEncode(string value)
        {
            if( string.IsNullOrWhiteSpace(value) ) return value;
            // Escape the ' (single quote)
            // Url encode data values
            var final = value.Replace("'", @"\'");
            return Uri.EscapeDataString(final);
        }
    }
}
