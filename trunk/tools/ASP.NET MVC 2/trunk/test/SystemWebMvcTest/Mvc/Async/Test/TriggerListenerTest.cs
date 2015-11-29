namespace System.Web.Mvc.Async.Test {
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TriggerListenerTest {

        [TestMethod]
        public void PerformTest() {
            // Arrange
            int count = 0;
            TriggerListener listener = new TriggerListener();
            Trigger trigger = listener.CreateTrigger();

            // Act & assert 1
            listener.SetContinuation(() => { count++; });
            listener.Activate();
            Assert.AreEqual(0, count, "Callback shouldn't have been executed.");

            // Act & assert 2
            trigger.Fire();
            Assert.AreEqual(1, count, "Callback should've been called once.");

            // Act & assert 3
            Trigger trigger2 = listener.CreateTrigger();
            trigger2.Fire();
            Assert.AreEqual(1, count, "Callback should only be called once.");
        }

    }
}
