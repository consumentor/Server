namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc.Resources;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class RedirectResultTest {

        private static string _baseUrl = "http://www.contoso.com/";

        [TestMethod]
        public void ConstructorSetsUrl() {
            // Act
            var result = new RedirectResult(_baseUrl);

            // Assert
            Assert.AreSame(_baseUrl, result.Url);
        }

        [TestMethod]
        public void ConstructorWithEmptyUrlThrows() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new RedirectResult(String.Empty);
                },
                "url");
        }

        [TestMethod]
        public void ConstructorWithNullUrlThrows() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new RedirectResult(null /* url */);
                },
                "url");
        }

        [TestMethod]
        public void ExecuteResultCallsResponseRedirect() {
            // Arrange
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>();
            mockResponse.Expect(o => o.Redirect(_baseUrl, false /* endResponse */));
            Mock<HttpContextBase> mockContext = new Mock<HttpContextBase>();
            mockContext.Expect(o => o.Response).Returns(mockResponse.Object);
            ControllerContext context = new ControllerContext(mockContext.Object, new RouteData(), new Mock<ControllerBase>().Object);
            var result = new RedirectResult(_baseUrl);

            // Act
            result.ExecuteResult(context);

            // Assert
            mockResponse.Verify();
        }

        [TestMethod]
        public void ExecuteResultWithNullControllerContextThrows() {
            // Arrange
            var result = new RedirectResult(_baseUrl);

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    result.ExecuteResult(null /* context */);
                },
                "context");
        }

        [TestMethod]
        public void RedirectInChildActionThrows() {
            // Arrange
            RouteData routeData = new RouteData();
            routeData.DataTokens[ControllerContext.PARENT_ACTION_VIEWCONTEXT] = new ViewContext();
            ControllerContext context = new ControllerContext(new Mock<HttpContextBase>().Object, routeData, new Mock<ControllerBase>().Object);
            RedirectResult result = new RedirectResult(_baseUrl);

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => result.ExecuteResult(context),
                MvcResources.RedirectAction_CannotRedirectInChildAction
            );
        }

    }
}
