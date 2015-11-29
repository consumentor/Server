namespace System.Web.Mvc.Test {
    using System;
    using System.IO;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class HttpHandlerUtilTest {

        [TestMethod]
        public void WrapForServerExecute_BeginProcessRequest_DelegatesCorrectly() {
            // Arrange
            IAsyncResult expectedResult = new Mock<IAsyncResult>().Object;
            AsyncCallback cb = delegate { };

            HttpContext httpContext = GetHttpContext();
            Mock<IHttpAsyncHandler> mockHttpHandler = new Mock<IHttpAsyncHandler>();
            mockHttpHandler.Expect(o => o.BeginProcessRequest(httpContext, cb, "extraData")).Returns(expectedResult);

            IHttpAsyncHandler wrapper = (IHttpAsyncHandler)HttpHandlerUtil.WrapForServerExecute(mockHttpHandler.Object);

            // Act
            IAsyncResult actualResult = wrapper.BeginProcessRequest(httpContext, cb, "extraData");

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void WrapForServerExecute_EndProcessRequest_DelegatesCorrectly() {
            // Arrange
            IAsyncResult asyncResult = new Mock<IAsyncResult>().Object;

            HttpContext httpContext = GetHttpContext();
            Mock<IHttpAsyncHandler> mockHttpHandler = new Mock<IHttpAsyncHandler>();
            mockHttpHandler.Expect(o => o.EndProcessRequest(asyncResult)).Verifiable();

            IHttpAsyncHandler wrapper = (IHttpAsyncHandler)HttpHandlerUtil.WrapForServerExecute(mockHttpHandler.Object);

            // Act
            wrapper.EndProcessRequest(asyncResult);

            // Assert
            mockHttpHandler.Verify();
        }

        [TestMethod]
        public void WrapForServerExecute_ProcessRequest_DelegatesCorrectly() {
            // Arrange
            HttpContext httpContext = GetHttpContext();
            Mock<IHttpHandler> mockHttpHandler = new Mock<IHttpHandler>();
            mockHttpHandler.Expect(o => o.ProcessRequest(httpContext)).Verifiable();

            IHttpHandler wrapper = HttpHandlerUtil.WrapForServerExecute(mockHttpHandler.Object);

            // Act
            wrapper.ProcessRequest(httpContext);

            // Assert
            mockHttpHandler.Verify();
        }

        [TestMethod]
        public void WrapForServerExecute_ProcessRequest_PropagatesExceptionsIfNotHttpException() {
            // Arrange
            HttpContext httpContext = GetHttpContext();
            Mock<IHttpHandler> mockHttpHandler = new Mock<IHttpHandler>();
            mockHttpHandler.Expect(o => o.ProcessRequest(httpContext)).Throws(new InvalidOperationException("Some exception."));

            IHttpHandler wrapper = HttpHandlerUtil.WrapForServerExecute(mockHttpHandler.Object);

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    wrapper.ProcessRequest(httpContext);
                },
                @"Some exception.");
        }

        [TestMethod]
        public void WrapForServerExecute_ProcessRequest_PropagatesHttpExceptionIfStatusCode500() {
            // Arrange
            HttpContext httpContext = GetHttpContext();
            Mock<IHttpHandler> mockHttpHandler = new Mock<IHttpHandler>();
            mockHttpHandler.Expect(o => o.ProcessRequest(httpContext)).Throws(new HttpException(500, "Some exception."));

            IHttpHandler wrapper = HttpHandlerUtil.WrapForServerExecute(mockHttpHandler.Object);

            // Act & assert
            ExceptionHelper.ExpectHttpException(
                delegate {
                    wrapper.ProcessRequest(httpContext);
                },
                @"Some exception.",
                500);
        }

        [TestMethod]
        public void WrapForServerExecute_ProcessRequest_WrapsHttpExceptionIfStatusCodeNot500() {
            // Arrange
            HttpContext httpContext = GetHttpContext();
            Mock<IHttpHandler> mockHttpHandler = new Mock<IHttpHandler>();
            mockHttpHandler.Expect(o => o.ProcessRequest(httpContext)).Throws(new HttpException(404, "Some exception."));

            IHttpHandler wrapper = HttpHandlerUtil.WrapForServerExecute(mockHttpHandler.Object);

            // Act & assert
            HttpException outerException = ExceptionHelper.ExpectHttpException(
                delegate {
                    wrapper.ProcessRequest(httpContext);
                },
                @"Execution of the child request failed. Please examine the InnerException for more information.",
                500);

            HttpException innerException = outerException.InnerException as HttpException;
            Assert.IsNotNull(innerException, "Inner exception should have been the original HttpException.");
            Assert.AreEqual(404, innerException.GetHttpCode());
            Assert.AreEqual("Some exception.", innerException.Message);
        }


        [TestMethod]
        public void WrapForServerExecute_ReturnsIHttpAsyncHandler() {
            // Arrange
            IHttpAsyncHandler httpHandler = new Mock<IHttpAsyncHandler>().Object;

            // Act
            IHttpHandler wrapper = HttpHandlerUtil.WrapForServerExecute(httpHandler);

            // Assert
            Assert.IsTrue(wrapper is IHttpAsyncHandler, "Wrapper should have been an async handler.");
        }

        [TestMethod]
        public void WrapForServerExecute_ReturnsIHttpHandler() {
            // Arrange
            IHttpHandler httpHandler = new Mock<IHttpHandler>().Object;

            // Act
            IHttpHandler wrapper = HttpHandlerUtil.WrapForServerExecute(httpHandler);

            // Assert
            Assert.IsFalse(wrapper is IHttpAsyncHandler, "Wrapper should have been only a sync handler.");
        }

        private static HttpContext GetHttpContext() {
            return new HttpContext(new SimpleWorkerRequest("/", "/", "Page", "Query", TextWriter.Null));
        }

    }
}
