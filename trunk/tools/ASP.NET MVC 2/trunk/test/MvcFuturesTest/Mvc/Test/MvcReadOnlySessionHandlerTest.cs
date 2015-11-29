namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Web;
    using System.Web.SessionState;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class MvcReadOnlySessionHandlerTest {

        [TestMethod]
        public void Handler_DoesNotRequireSessionState() {
            // Arrange
            MvcReadOnlySessionHandler handler = new MvcReadOnlySessionHandler(new Mock<IHttpAsyncHandler>().Object);

            // Act & assert
            Assert.IsTrue(handler is IReadOnlySessionState, "The wrapping handler should require read-only Session.");
        }

    }
}
