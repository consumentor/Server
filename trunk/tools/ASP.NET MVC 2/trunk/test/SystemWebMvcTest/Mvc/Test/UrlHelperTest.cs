namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class UrlHelperTest {
        [TestMethod]
        public void RequestContextProperty() {
            // Arrange
            RequestContext requestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());
            UrlHelper urlHelper = new UrlHelper(requestContext);

            // Assert
            Assert.AreEqual(requestContext, urlHelper.RequestContext);
        }

        [TestMethod]
        public void ConstructorWithNullRequestContextThrows() {
            // Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new UrlHelper(null);
                },
                "requestContext");
        }

        [TestMethod]
        public void ConstructorWithNullRouteCollectionThrows() {
            // Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new UrlHelper(GetRequestContext(), null);
                },
                "routeCollection");
        }

        [TestMethod]
        public void Action() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.Action("newaction");

            // Assert
            Assert.AreEqual<string>(MvcHelper.AppPathModifier + "/app/home/newaction", url);
        }

        [TestMethod]
        public void ActionWithControllerName() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.Action("newaction", "home2");

            // Assert
            Assert.AreEqual<string>(MvcHelper.AppPathModifier + "/app/home2/newaction", url);
        }

        [TestMethod]
        public void ActionWithControllerNameAndDictionary() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.Action("newaction", "home2", new RouteValueDictionary(new { id = "someid" }));

            // Assert
            Assert.AreEqual<string>(MvcHelper.AppPathModifier + "/app/home2/newaction/someid", url);
        }

        [TestMethod]
        public void ActionWithControllerNameAndObjectProperties() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.Action("newaction", "home2", new { id = "someid" });

            // Assert
            Assert.AreEqual<string>(MvcHelper.AppPathModifier + "/app/home2/newaction/someid", url);
        }

        [TestMethod]
        public void ActionWithDictionary() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.Action("newaction", new RouteValueDictionary(new { Controller = "home2", id = "someid" }));

            // Assert
            Assert.AreEqual<string>(MvcHelper.AppPathModifier + "/app/home2/newaction/someid", url);
        }

        [TestMethod]
        public void ActionWithNullActionName() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.Action(null);

            // Assert
            Assert.AreEqual(MvcHelper.AppPathModifier + "/app/home/oldaction", url);
        }

        [TestMethod]
        public void ActionWithNullProtocol() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.Action("newaction", "home2", new { id = "someid" }, null /* protocol */);

            // Assert
            Assert.AreEqual<string>(MvcHelper.AppPathModifier + "/app/home2/newaction/someid", url);
        }

        [TestMethod]
        public void ActionParameterOverridesObjectProperties() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.Action("newaction", new { Action = "action" });

            // Assert
            Assert.AreEqual<string>(MvcHelper.AppPathModifier + "/app/home/newaction", url);
        }

        [TestMethod]
        public void ActionWithObjectProperties() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.Action("newaction", new { Controller = "home2", id = "someid" });

            // Assert
            Assert.AreEqual<string>(MvcHelper.AppPathModifier + "/app/home2/newaction/someid", url);
        }

        [TestMethod]
        public void ActionWithProtocol() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.Action("newaction", "home2", new { id = "someid" }, "https");

            // Assert
            Assert.AreEqual<string>("https://localhost" + MvcHelper.AppPathModifier + "/app/home2/newaction/someid", url);
        }

        [TestMethod]
        public void ContentWithAbsolutePath() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.Content("/Content/Image.jpg");

            // Assert
            Assert.AreEqual("/Content/Image.jpg", url);
        }

        [TestMethod]
        public void ContentWithAppRelativePath() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.Content("~/Content/Image.jpg");

            // Assert
            Assert.AreEqual(MvcHelper.AppPathModifier + "/app/Content/Image.jpg", url);
        }

        [TestMethod]
        public void ContentWithEmptyPathThrows() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate() {
                    urlHelper.Content(String.Empty);
                },
                "contentPath");
        }

        [TestMethod]
        public void ContentWithRelativePath() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.Content("Content/Image.jpg");

            // Assert
            Assert.AreEqual("Content/Image.jpg", url);
        }

        [TestMethod]
        public void ContentWithExternalUrl() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.Content("http://www.asp.net/App_Themes/Standard/i/logo.png");

            // Assert
            Assert.AreEqual("http://www.asp.net/App_Themes/Standard/i/logo.png", url);
        }

        [TestMethod]
        public void Encode() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string encodedUrl = urlHelper.Encode(@"SomeUrl /+\");

            // Assert
            Assert.AreEqual(encodedUrl, "SomeUrl+%2f%2b%5c");
        }

        [TestMethod]
        public void GenerateContentUrlWithNullContentPathThrows() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate() {
                    UrlHelper.GenerateContentUrl(null, null);
                },
                "contentPath");
        }

        [TestMethod]
        public void GenerateContentUrlWithNullContextThrows() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate() {
                    UrlHelper.GenerateContentUrl("Content/foo.png", null);
                },
                "httpContext");
        }

        [TestMethod]
        public void GenerateUrlWithNullRequestContextThrows() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate() {
                    UrlHelper.GenerateUrl(null /* routeName */, null /* actionName */, null /* controllerName */, null /* routeValues */, new RouteCollection(), null /* requestContext */, false);
                },
                "requestContext");
        }

        [TestMethod]
        public void GenerateUrlWithNullRouteCollectionThrows() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate() {
                    UrlHelper.GenerateUrl(null /* routeName */, null /* actionName */, null /* controllerName */, null /* routeValues */, null /* routeCollection */, null /* requestContext */, false);
                },
                "routeCollection");
        }

        [TestMethod]
        public void GenerateUrlWithEmptyCollectionsReturnsNull() {
            // Arrange
            RequestContext requestContext = GetRequestContext();

            // Act
            string url = UrlHelper.GenerateUrl(null, null, null, null, new RouteCollection(), requestContext, true);

            // Assert
            Assert.IsNull(url);
        }

        [TestMethod]
        public void GenerateUrlWithAction() {
            // Arrange
            RequestContext requestContext = GetRequestContext(GetRouteData());

            // Act
            string url = UrlHelper.GenerateUrl(null, "newaction", null, null, GetRouteCollection(), requestContext, true);

            // Assert
            Assert.AreEqual(MvcHelper.AppPathModifier + "/app/home/newaction", url);
        }

        [TestMethod]
        public void GenerateUrlWithActionAndController() {
            // Arrange
            RequestContext requestContext = GetRequestContext(GetRouteData());

            // Act
            string url = UrlHelper.GenerateUrl(null, "newaction", "newcontroller", null, GetRouteCollection(), requestContext, true);

            // Assert
            Assert.AreEqual(MvcHelper.AppPathModifier + "/app/newcontroller/newaction", url);
        }

        [TestMethod]
        public void GenerateUrlWithImplicitValues() {
            // Arrange
            RequestContext requestContext = GetRequestContext(GetRouteData());

            // Act
            string url = UrlHelper.GenerateUrl(null, null, null, null, GetRouteCollection(), requestContext, true);

            // Assert
            Assert.AreEqual(MvcHelper.AppPathModifier + "/app/home/oldaction", url);
        }

        [TestMethod]
        public void RouteUrlCanUseNamedRouteWithoutSpecifyingDefaults() {
            // DevDiv 217072: Non-mvc specific helpers should not give default values for controller and action

            // Arrange
            UrlHelper urlHelper = GetUrlHelper();
            urlHelper.RouteCollection.MapRoute("MyRouteName", "any/url", new { controller = "Charlie" });

            // Act
            string result = urlHelper.RouteUrl("MyRouteName");

            // Assert
            Assert.AreEqual(MvcHelper.AppPathModifier + "/app/any/url", result);
        }

        [TestMethod]
        public void RouteUrlWithDictionary() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.RouteUrl(new RouteValueDictionary(new { Action = "newaction", Controller = "home2", id = "someid" }));

            // Assert
            Assert.AreEqual<string>(MvcHelper.AppPathModifier + "/app/home2/newaction/someid", url);
        }

        [TestMethod]
        public void RouteUrlWithEmptyHostName() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.RouteUrl("namedroute", new RouteValueDictionary(new { Action = "newaction", Controller = "home2", id = "someid" }), "http", String.Empty /* hostName */);

            // Assert
            Assert.AreEqual<string>("http://localhost" + MvcHelper.AppPathModifier + "/app/named/home2/newaction/someid", url);
        }

        [TestMethod]
        public void RouteUrlWithEmptyProtocol() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.RouteUrl("namedroute", new RouteValueDictionary(new { Action = "newaction", Controller = "home2", id = "someid" }), String.Empty /* protocol */, "foo.bar.com");

            // Assert
            Assert.AreEqual<string>("http://foo.bar.com" + MvcHelper.AppPathModifier + "/app/named/home2/newaction/someid", url);
        }

        [TestMethod]
        public void RouteUrlWithNullProtocol() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.RouteUrl("namedroute", new RouteValueDictionary(new { Action = "newaction", Controller = "home2", id = "someid" }), null /* protocol */, "foo.bar.com");

            // Assert
            Assert.AreEqual<string>("http://foo.bar.com" + MvcHelper.AppPathModifier + "/app/named/home2/newaction/someid", url);
        }

        [TestMethod]
        public void RouteUrlWithNullProtocolAndNullHostName() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.RouteUrl("namedroute", new RouteValueDictionary(new { Action = "newaction", Controller = "home2", id = "someid" }), null /* protocol */, null /* hostName */);

            // Assert
            Assert.AreEqual<string>(MvcHelper.AppPathModifier + "/app/named/home2/newaction/someid", url);
        }

        [TestMethod]
        public void RouteUrlWithObjectProperties() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.RouteUrl(new { Action = "newaction", Controller = "home2", id = "someid" });

            // Assert
            Assert.AreEqual<string>(MvcHelper.AppPathModifier + "/app/home2/newaction/someid", url);
        }

        [TestMethod]
        public void RouteUrlWithProtocol() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.RouteUrl("namedroute", new { Action = "newaction", Controller = "home2", id = "someid" }, "https");

            // Assert
            Assert.AreEqual<string>("https://localhost" + MvcHelper.AppPathModifier + "/app/named/home2/newaction/someid", url);
        }

        [TestMethod]
        public void RouteUrlWithRouteNameAndDefaults() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.RouteUrl("namedroute");

            // Assert
            Assert.AreEqual<string>(MvcHelper.AppPathModifier + "/app/named/home/oldaction", url);
        }

        [TestMethod]
        public void RouteUrlWithRouteNameAndDictionary() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.RouteUrl("namedroute", new RouteValueDictionary(new { Action = "newaction", Controller = "home2", id = "someid" }));

            // Assert
            Assert.AreEqual<string>(MvcHelper.AppPathModifier + "/app/named/home2/newaction/someid", url);
        }

        [TestMethod]
        public void RouteUrlWithRouteNameAndObjectProperties() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();

            // Act
            string url = urlHelper.RouteUrl("namedroute", new { Action = "newaction", Controller = "home2", id = "someid" });

            // Assert
            Assert.AreEqual<string>(MvcHelper.AppPathModifier + "/app/named/home2/newaction/someid", url);
        }

        [TestMethod]
        public void UrlGenerationDoesNotChangeProvidedDictionary() {
            // Arrange
            UrlHelper urlHelper = GetUrlHelper();
            RouteValueDictionary valuesDictionary = new RouteValueDictionary();

            // Act
            urlHelper.Action("actionName", valuesDictionary);

            // Assert
            Assert.AreEqual(0, valuesDictionary.Count);
            Assert.IsFalse(valuesDictionary.ContainsKey("action"));
        }

        private static RequestContext GetRequestContext() {
            HttpContextBase httpcontext = MvcHelper.GetHttpContext("/app/", null, null);
            RouteData rd = new RouteData();
            return new RequestContext(httpcontext, rd);
        }

        private static RequestContext GetRequestContext(RouteData routeData) {
            HttpContextBase httpcontext = MvcHelper.GetHttpContext("/app/", null, null);
            return new RequestContext(httpcontext, routeData);
        }

        private static RouteCollection GetRouteCollection() {
            RouteCollection rt = new RouteCollection();
            rt.Add(new Route("{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            rt.Add("namedroute", new Route("named/{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            return rt;
        }

        private static RouteData GetRouteData() {
            RouteData rd = new RouteData();
            rd.Values.Add("controller", "home");
            rd.Values.Add("action", "oldaction");
            return rd;
        }

        private static UrlHelper GetUrlHelper() {
            HttpContextBase httpcontext = MvcHelper.GetHttpContext("/app/", null, null);
            UrlHelper urlHelper = new UrlHelper(new RequestContext(httpcontext, GetRouteData()), GetRouteCollection());
            return urlHelper;
        }
    }
}
