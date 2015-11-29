namespace System.Web.Mvc.Test {
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpRequestExtensionsTest {
        [TestMethod]
        public void GetHttpMethodOverrideWithNullRequestThrows() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    HttpRequestExtensions.GetHttpMethodOverride(null);
                }, "request");
        }

        // -----------------------------------------------------------------
        // Override scenarios: Overriding stuff you can't override
        // -----------------------------------------------------------------

        [TestMethod]
        public void CannotOverrideGetWithHeaderPut() {
            TestHttpMethodOverride("GET", "PUT", null, null, "GET");
        }

        [TestMethod]
        public void CannotOverrideGetWithFormPut() {
            TestHttpMethodOverride("GET", null, "PUT", null, "GET");
        }

        [TestMethod]
        public void CannotOverrideGetWithQueryStringPut() {
            TestHttpMethodOverride("GET", null, null, "PUT", "GET");
        }

        [TestMethod]
        public void CannotOverridePutWithGet() {
            TestHttpMethodOverride("PUT", "GET", null, null, "PUT");
        }

        [TestMethod]
        public void CannotOverridePutWithPost() {
            TestHttpMethodOverride("PUT", "POST", null, null, "PUT");
        }

        [TestMethod]
        public void CannotOverridePostWithGet() {
            TestHttpMethodOverride("POST", "GET", null, null, "POST");
        }

        [TestMethod]
        public void CannotOverridePostWithPost() {
            TestHttpMethodOverride("POST", "POST", null, null, "POST");
        }

        // -----------------------------------------------------------------
        // Override scenarios: Overriding stuff you can override
        // -----------------------------------------------------------------

        [TestMethod]
        public void CanOverridePostWithHeaderPut() {
            TestHttpMethodOverride("POST", "PUT", null, null, "PUT");
        }

        [TestMethod]
        public void CanOverridePostWithFormPut() {
            TestHttpMethodOverride("POST", null, "PUT", null, "PUT");
        }

        [TestMethod]
        public void CanOverridePostWithQueryStringPut() {
            TestHttpMethodOverride("POST", null, null, "PUT", "PUT");
        }

        // -----------------------------------------------------------------
        // Override scenarios: Precedence scenarios
        // -----------------------------------------------------------------

        [TestMethod]
        public void HeaderOverrideWinsOverFormOverride() {
            TestHttpMethodOverride("POST", "PUT", "BOGUS", null, "PUT");
        }

        [TestMethod]
        public void HeaderOverrideWinsOverQueryStringOverride() {
            TestHttpMethodOverride("POST", "PUT", null, "BOGUS", "PUT");
        }

        [TestMethod]
        public void FormOverrideWinsOverQueryStringOverride() {
            TestHttpMethodOverride("POST", null, "PUT", "BOGUS", "PUT");
        }

        private static void TestHttpMethodOverride(string httpRequestVerb, string httpHeaderVerb, string httpFormVerb, string httpQueryStringVerb, string expectedMethod) {
            // Arrange
            ControllerContext context = AcceptVerbsAttributeTest.GetControllerContextWithHttpVerb(httpRequestVerb, httpHeaderVerb, httpFormVerb, httpQueryStringVerb);

            // Act
            string methodOverride = context.RequestContext.HttpContext.Request.GetHttpMethodOverride();

            // Assert
            Assert.AreEqual<string>(expectedMethod, methodOverride);
        }
    }
}
