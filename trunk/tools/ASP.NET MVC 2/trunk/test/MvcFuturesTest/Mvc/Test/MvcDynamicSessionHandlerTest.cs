namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.SessionState;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class MvcDynamicSessionHandlerTest {

        [TestMethod]
        public void BeginProcessRequest_ForwardsCall() {
            // Arrange
            HttpContext httpContext = new HttpContext(new SimpleWorkerRequest("/", "/", "Page", "Query", TextWriter.Null));
            AsyncCallback callback = ar => { };
            object extraData = new object();
            IAsyncResult asyncResult = new Mock<IAsyncResult>().Object;

            Mock<IHttpAsyncHandler> mockHandler = new Mock<IHttpAsyncHandler>();
            mockHandler.Expect(o => o.BeginProcessRequest(httpContext, callback, extraData)).Returns(asyncResult);

            MvcDynamicSessionHandler handler = new MvcDynamicSessionHandler(mockHandler.Object);

            // Act
            IAsyncResult retVal = handler.BeginProcessRequest(httpContext, callback, extraData);

            // Assert
            Assert.AreEqual(asyncResult, retVal);
        }

        [TestMethod]
        public void EndProcessRequest_ForwardsCall() {
            // Arrange
            IAsyncResult asyncResult = new Mock<IAsyncResult>().Object;

            Mock<IHttpAsyncHandler> mockHandler = new Mock<IHttpAsyncHandler>();
            mockHandler.Expect(o => o.EndProcessRequest(asyncResult)).Verifiable();

            MvcDynamicSessionHandler handler = new MvcDynamicSessionHandler(mockHandler.Object);

            // Act
            handler.EndProcessRequest(asyncResult);

            // Assert
            mockHandler.Verify();
        }

        [TestMethod]
        public void Handler_DoesNotRequireSessionState() {
            // Arrange
            MvcDynamicSessionHandler handler = new MvcDynamicSessionHandler(new Mock<IHttpAsyncHandler>().Object);

            // Act & assert
            Assert.IsFalse(handler is IRequiresSessionState, "The wrapping handler should not require Session.");
        }

        [TestMethod]
        public void IsReusable_ForwardsCall() {
            // Arrange
            IAsyncResult asyncResult = new Mock<IAsyncResult>().Object;

            Mock<IHttpAsyncHandler> mockHandler = new Mock<IHttpAsyncHandler>();
            mockHandler.Expect(o => o.IsReusable).Returns(true);

            MvcDynamicSessionHandler handler = new MvcDynamicSessionHandler(mockHandler.Object);

            // Act
            handler.EndProcessRequest(asyncResult);

            // Assert
            mockHandler.Verify();
        }

        [TestMethod]
        public void ProcessRequest_ForwardsCall() {
            // Arrange
            HttpContext httpContext = new HttpContext(new SimpleWorkerRequest("/", "/", "Page", "Query", TextWriter.Null));

            Mock<IHttpAsyncHandler> mockHandler = new Mock<IHttpAsyncHandler>();
            mockHandler.Expect(o => o.ProcessRequest(httpContext)).Verifiable();

            MvcDynamicSessionHandler handler = new MvcDynamicSessionHandler(mockHandler.Object);

            // Act
            handler.ProcessRequest(httpContext);

            // Assert
            mockHandler.Verify();
        }

    }
}
