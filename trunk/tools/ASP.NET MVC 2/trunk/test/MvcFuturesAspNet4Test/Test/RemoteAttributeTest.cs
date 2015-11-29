namespace Microsoft.Web.Mvc.AspNet4.Test {
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class RemoteAttributeTest {
        // Good route name, bad route name
        // Controller + Action

        [TestMethod]
        public void GuardClauses() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => new RemoteAttribute(null, "controller", "parameter"),
                "action");
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => new RemoteAttribute("action", null, "parameter"),
                "controller");
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => new RemoteAttribute("action", "controller", null),
                "parameterName");

            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => new RemoteAttribute(null, "parameter"),
                "routeName");
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => new RemoteAttribute("route", null),
                "parameterName");
        }

        [TestMethod]
        public void IsValidAlwaysReturnsTrue() {
            // Act & Assert
            Assert.IsTrue(new RemoteAttribute("RouteName", "ParameterName").IsValid(null));
            Assert.IsTrue(new RemoteAttribute("ActionName", "ControllerName", "ParameterName").IsValid(null));
        }

        [TestMethod]
        public void BadRouteNameThrows() {
            // Arrange
            ControllerContext context = new ControllerContext();
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(object));
            TestableRemoteAttribute attribute = new TestableRemoteAttribute("RouteName", "ParameterName");

            // Act & Assert
            ExceptionHelper.ExpectArgumentException(
                () => attribute.GetClientValidationRules(metadata, context),
                "A route named 'RouteName' could not be found in the route collection.\r\nParameter name: name");
        }

        [TestMethod]
        public void GoodRouteNameReturnsCorrectClientData() {
            // Arrange
            string url = null;
            Mock<ControllerContext> context = new Mock<ControllerContext>();
            context.Expect(c => c.HttpContext.Request.ApplicationPath)
                   .Returns("/");
            context.Expect(c => c.HttpContext.Response.ApplyAppPathModifier(It.IsAny<string>()))
                   .Callback<string>(vpath => url = vpath)
                   .Returns(() => url);
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(string), "Length");
            TestableRemoteAttribute attribute = new TestableRemoteAttribute("RouteName", "ParameterName");
            attribute.RouteTable.Add("RouteName", new Route("my/url", new MvcRouteHandler()));

            // Act
            ModelClientValidationRule rule = attribute.GetClientValidationRules(metadata, context.Object).Single();

            // Assert
            Assert.AreEqual("remote", rule.ValidationType);
            Assert.AreEqual("The field Length is invalid.", rule.ErrorMessage);
            Assert.AreEqual(2, rule.ValidationParameters.Count);
            Assert.AreEqual(url, rule.ValidationParameters["url"]);
            Assert.AreEqual("ParameterName", rule.ValidationParameters["parameterName"]);
        }

        [TestMethod]
        public void NoRouteWithActionControllerThrows() {
            // Arrange
            ControllerContext context = new ControllerContext();
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(string), "Length");
            TestableRemoteAttribute attribute = new TestableRemoteAttribute("Action", "Controller", "ParameterName");

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => attribute.GetClientValidationRules(metadata, context),
                "No route matched!");
        }

        [TestMethod]
        public void ActionControllerReturnsCorrectClientData() {
            // Arrange
            string url = null;
            Mock<ControllerContext> context = new Mock<ControllerContext>();
            context.Expect(c => c.HttpContext.Request.ApplicationPath)
                   .Returns("/");
            context.Expect(c => c.HttpContext.Response.ApplyAppPathModifier(It.IsAny<string>()))
                   .Callback<string>(vpath => url = vpath)
                   .Returns(() => url);
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(string), "Length");
            TestableRemoteAttribute attribute = new TestableRemoteAttribute("Action", "Controller", "ParameterName");
            attribute.RouteTable.Add(new Route("{controller}/{action}", new MvcRouteHandler()));

            // Act
            ModelClientValidationRule rule = attribute.GetClientValidationRules(metadata, context.Object).Single();

            // Assert
            Assert.AreEqual("remote", rule.ValidationType);
            Assert.AreEqual("The field Length is invalid.", rule.ErrorMessage);
            Assert.AreEqual(2, rule.ValidationParameters.Count);
            Assert.AreEqual(url, rule.ValidationParameters["url"]);
            Assert.AreEqual("ParameterName", rule.ValidationParameters["parameterName"]);
        }

        private class TestableRemoteAttribute : RemoteAttribute {
            public RouteCollection RouteTable = new RouteCollection();

            public TestableRemoteAttribute(string action, string controller, string parameterName)
                : base(action, controller, parameterName) { }

            public TestableRemoteAttribute(string routeName, string parameterName)
                : base(routeName, parameterName) { }

            protected override RouteCollection Routes {
                get {
                    return RouteTable;
                }
            }
        }
    }
}
