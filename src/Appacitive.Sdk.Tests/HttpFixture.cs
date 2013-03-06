using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class HttpFixture
    {
        [TestMethod]
        public void GetTest()
        {
            var waitHandle = new ManualResetEvent(false);
            GetAsync(waitHandle);
            Console.WriteLine("Waiting for get method to return.");
            waitHandle.WaitOne();
            Console.WriteLine("Test completed.");
        }

        private async void GetAsync(ManualResetEvent waitHandle)
        {
            try
            {
                var content = await HttpOperation
                    .WithUrl("http://www.google.co.in")
                    .GetAsync();
                Console.WriteLine(Encoding.UTF8.GetString(content));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                waitHandle.Set();
            }

        }

    }
}
