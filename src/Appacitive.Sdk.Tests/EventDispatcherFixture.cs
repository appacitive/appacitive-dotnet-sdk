using Appacitive.Sdk.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class EventDispatcherFixture
    {
        [TestMethod]
        public void RegisterEventForTypeAndInvoke()
        {
            var invoked = false;
            IEventDispatcher dispatcher = ObjectFactory.Build<IEventDispatcher>();
            dispatcher.Subscribe("test", "test", m =>
                {
                    invoked = true;
                });
            dispatcher.Notify(new ObjectUpdatedMessage
            {
                EventType = "test",
                Type = "test"
            });
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void RegisterEventForTypeAndIdInvoke()
        {
            var invoked = false;
            IEventDispatcher dispatcher = ObjectFactory.Build<IEventDispatcher>();
            dispatcher.Subscribe("test", "test", "1", m =>
            {
                invoked = true;
            });
            dispatcher.Notify(new ObjectUpdatedMessage
            {
                EventType = "test",
                Type = "test",
                ObjectId = "1"
            });
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void TestForDoubleInvocation()
        {
            var typeInvoked = false;
            var typeAndIdInvoked = false;
            IEventDispatcher dispatcher = ObjectFactory.Build<IEventDispatcher>();
            dispatcher.Subscribe("test", "test", m =>
            {
                typeInvoked = true;
            });
            dispatcher.Subscribe("test", "test", "1", m =>
            {
                typeAndIdInvoked = true;
            });
            dispatcher.Notify(new ObjectUpdatedMessage
            {
                EventType = "test",
                Type = "test",
                ObjectId = "1"
            });
            Assert.IsTrue(typeInvoked);
            Assert.IsTrue(typeAndIdInvoked);
        }

        [TestMethod]
        public void DoubleRegistrationShouldFailTest()
        {
            Articles.Subscribe("update", "user", e =>
                {
                   
                });
            IEventDispatcher dispatcher = ObjectFactory.Build<IEventDispatcher>();
            dispatcher.Subscribe("test", "test", m =>
            {   
                // Do something
            });
            try
            {
                dispatcher.Subscribe("test", "test", m =>
                {
                    // Do something
                });
                Assert.Fail("Subscription did not fail on multiple subscriptions to same event.");
            }
            catch
            {
            }
        }
    }
}
