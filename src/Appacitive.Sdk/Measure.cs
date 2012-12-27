using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static class Measure
    {
        public static decimal TimeFor(Action action)
        {
            decimal timeTaken = decimal.Zero;
            var timer = Stopwatch.StartNew();
            try
            {
                action();
            }
            finally
            {
                timer.Stop();
                timeTaken = (decimal)timer.ElapsedTicks / Stopwatch.Frequency;
                timer = null;
            }
            return timeTaken;
        }
    }
}
