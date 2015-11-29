namespace System.Web.Mvc.Async.Test {
    using System;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SingleEntryGateTest {

        [TestMethod]
        public void TryEnter() {
            // Arrange
            SingleEntryGate gate = new SingleEntryGate();

            // Act
            bool firstCall = gate.TryEnter();
            bool secondCall = gate.TryEnter();
            bool thirdCall = gate.TryEnter();

            // Assert
            Assert.IsTrue(firstCall, "TryEnter() should return TRUE on first call.");
            Assert.IsFalse(secondCall, "TryEnter() should return FALSE on each subsequent call.");
            Assert.IsFalse(thirdCall, "TryEnter() should return FALSE on each subsequent call.");
        }

    }
}
