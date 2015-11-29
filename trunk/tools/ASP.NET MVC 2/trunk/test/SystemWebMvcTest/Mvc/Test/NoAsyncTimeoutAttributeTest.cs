namespace System.Web.Mvc.Test {
    using System;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NoAsyncTimeoutAttributeTest {

        [TestMethod]
        public void DurationPropertyIsZero() {
            // Act
            AsyncTimeoutAttribute attr = new NoAsyncTimeoutAttribute();

            // Assert
            Assert.AreEqual(Timeout.Infinite, attr.Duration);
        }

    }
}
