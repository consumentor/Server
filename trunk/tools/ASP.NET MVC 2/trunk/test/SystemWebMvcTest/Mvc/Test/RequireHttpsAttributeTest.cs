namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class RequireHttpsAttributeTest {

        [TestMethod]
        public void HandleNonHttpsRequestExtensibility() {
            // Arrange
            Mock<AuthorizationContext> mockAuthContext = new Mock<AuthorizationContext>();
            mockAuthContext.Expect(c => c.HttpContext.Request.IsSecureConnection).Returns(false);
            AuthorizationContext authContext = mockAuthContext.Object;

            RequireHttpsAttribute attr = new MyRequireHttpsAttribute();

            // Act
            attr.OnAuthorization(authContext);
            ContentResult result = authContext.Result as ContentResult;

            // Assert
            Assert.IsNotNull(result, "Result should have been a ContentResult.");
            Assert.AreEqual("Custom HandleNonHttpsRequest", result.Content);
        }

        [TestMethod]
        public void OnAuthorizationDoesNothingIfRequestIsSecure() {
            // Arrange
            Mock<AuthorizationContext> mockAuthContext = new Mock<AuthorizationContext>();
            mockAuthContext.Expect(c => c.HttpContext.Request.IsSecureConnection).Returns(true);
            AuthorizationContext authContext = mockAuthContext.Object;

            ViewResult result = new ViewResult();
            authContext.Result = result;

            RequireHttpsAttribute attr = new RequireHttpsAttribute();

            // Act
            attr.OnAuthorization(authContext);

            // Assert
            Assert.AreSame(result, authContext.Result, "Result should not have been changed.");
        }

        [TestMethod]
        public void OnAuthorizationRedirectsIfRequestIsNotSecureAndMethodIsGet() {
            // Arrange
            Mock<AuthorizationContext> mockAuthContext = new Mock<AuthorizationContext>();
            mockAuthContext.Expect(c => c.HttpContext.Request.HttpMethod).Returns("get");
            mockAuthContext.Expect(c => c.HttpContext.Request.IsSecureConnection).Returns(false);
            mockAuthContext.Expect(c => c.HttpContext.Request.RawUrl).Returns("/alpha/bravo/charlie?q=quux");
            mockAuthContext.Expect(c => c.HttpContext.Request.Url).Returns(new Uri("http://www.example.com:8080/foo/bar/baz"));
            AuthorizationContext authContext = mockAuthContext.Object;

            RequireHttpsAttribute attr = new RequireHttpsAttribute();

            // Act
            attr.OnAuthorization(authContext);
            RedirectResult result = authContext.Result as RedirectResult;

            // Assert
            Assert.IsNotNull(result, "Result should have been a RedirectResult.");
            Assert.AreEqual("https://www.example.com/alpha/bravo/charlie?q=quux", result.Url);
        }

        [TestMethod]
        public void OnAuthorizationThrowsIfFilterContextIsNull() {
            // Arrange
            RequireHttpsAttribute attr = new RequireHttpsAttribute();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    attr.OnAuthorization(null);
                }, "filterContext");
        }

        [TestMethod]
        public void OnAuthorizationThrowsIfRequestIsNotSecureAndMethodIsNotGet() {
            // Arrange
            Mock<AuthorizationContext> mockAuthContext = new Mock<AuthorizationContext>();
            mockAuthContext.Expect(c => c.HttpContext.Request.HttpMethod).Returns("post");
            mockAuthContext.Expect(c => c.HttpContext.Request.IsSecureConnection).Returns(false);
            AuthorizationContext authContext = mockAuthContext.Object;

            RequireHttpsAttribute attr = new RequireHttpsAttribute();

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    attr.OnAuthorization(authContext);
                },
                @"The requested resource can only be accessed via SSL.");
        }

        private class MyRequireHttpsAttribute : RequireHttpsAttribute {
            protected override void HandleNonHttpsRequest(AuthorizationContext filterContext) {
                filterContext.Result = new ContentResult() { Content = "Custom HandleNonHttpsRequest" };
            }
        }

    }
}
