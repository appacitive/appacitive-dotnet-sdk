using Appacitive.Sdk.Interfaces;
using Appacitive.Sdk.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class ExceptionFactoryFixture
    {
        [TestMethod]
        public void WindowsExceptionFactoryTest()
        {
            App.Initialize( WindowsRT.Host, TestConfiguration.ApiKey, TestConfiguration.Environment );
            var status = new Status()
                {
                    Code = "400",
                    Message = "Test exception",
                    FaultType = "test fault",
                    ReferenceId = Guid.NewGuid().ToString()
                };
            var exceptionFactory = App.Factory.Build<IExceptionFactory>();
            var fault = exceptionFactory.CreateFault(status);
            Assert.IsNotNull(fault);
            Assert.IsInstanceOfType(fault, typeof(Appacitive.Sdk.WinRT.AppacitiveException));
        }
    }
}
