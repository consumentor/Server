namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc.Resources;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class RedirectToRouteResultTest {

        [TestMethod]
        public void ConstructorWithNullValuesDictionary() {
            // Act
            var result = new RedirectToRouteResult(null /* routeValues */);

            // Assert
            Assert.IsNotNull(result.RouteValues);
            Assert.AreEqual<int>(0, result.RouteValues.Count);
            Assert.AreEqual<string>(String.Empty, result.RouteName);
        }

        [TestMethod]
        public void ConstructorSetsValuesDictionary() {
            // Arrange
            RouteValueDictionary dict = new RouteValueDictionary();

            // Act
            var result = new RedirectToRouteResult(dict);

            // Assert
            Assert.AreSame(dict, result.RouteValues);
            Assert.AreEqual<string>(String.Empty, result.RouteName);
        }

        [TestMethod]
        public void ConstructorSetsValuesDictionaryAndEmptyName() {
            // Arrange
            RouteValueDictionary dict = new RouteValueDictionary();

            // Act
            var result = new RedirectToRouteResult(null, dict);

            // Assert
            Assert.AreSame(dict, result.RouteValues);
            Assert.AreEqual<string>(String.Empty, result.RouteName);
        }

        [TestMethod]
        public void ConstructorSetsValuesDictionaryAndName() {
            // Arrange
            RouteValueDictionary dict = new RouteValueDictionary();

            // Act
            var result = new RedirectToRouteResult("foo", dict);

            // Assert
            Assert.AreSame(dict, result.RouteValues);
            Assert.AreEqual<string>("foo", result.RouteName);
        }

        [TestMethod]
        public void ExecuteResult() {
            // Arrange
            Mock<Controller> mockController = new Mock<Controller>();
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.HttpContext.Request.ApplicationPath).Returns("/somepath");
            mockControllerContext.Expect(c => c.HttpContext.Response.ApplyAppPathModifier(It.IsAny<string>())).Returns((string s) => s);
            mockControllerContext.Expect(c => c.HttpContext.Response.Redirect("/somepath/c/a/i", false)).Verifiable();
            mockControllerContext.Expect(c => c.Controller).Returns(mockController.Object);

            var values = new { Controller = "c", Action = "a", Id = "i" };
            RedirectToRouteResult result = new RedirectToRouteResult(new RouteValueDictionary(values)) {
                Routes = new RouteCollection() { new Route("{controller}/{action}/{id}", null) },
            };

            // Act
            result.ExecuteResult(mockControllerContext.Object);

            // Assert
            mockControllerContext.Verify();
        }

        [TestMethod]
        public void ExecuteResultPreservesTempData() {
            // Arrange
            TempDataDictionary tempData = new TempDataDictionary();
            tempData["Foo"] = "Foo";
            tempData["Bar"] = "Bar";
            Mock<Controller> mockController = new Mock<Controller>() { CallBase = true };
            mockController.Object.TempData = tempData;
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.HttpContext.Request.ApplicationPath).Returns("/somepath");
            mockControllerContext.Expect(c => c.HttpContext.Response.ApplyAppPathModifier(It.IsAny<string>())).Returns((string s) => s);
            mockControllerContext.Expect(c => c.HttpContext.Response.Redirect("/somepath/c/a/i", false)).Verifiable();
            mockControllerContext.Expect(c => c.Controller).Returns(mockController.Object);

            var values = new { Controller = "c", Action = "a", Id = "i" };
            RedirectToRouteResult result = new RedirectToRouteResult(new RouteValueDictionary(values)) {
                Routes = new RouteCollection() { new Route("{controller}/{action}/{id}", null) },
            };

            // Act
            object value = tempData["Foo"];
            result.ExecuteResult(mockControllerContext.Object);
            mockController.Object.TempData.Save(mockControllerContext.Object, new Mock<ITempDataProvider>().Object);

            // Assert
            Assert.IsTrue(tempData.ContainsKey("Foo"));
            Assert.IsTrue(tempData.ContainsKey("Bar"));
        }

        [TestMethod]
        public void ExecuteResultThrowsIfVirtualPathDataIsNull() {
            // Arrange
            var result = new RedirectToRouteResult(null) {
                Routes = new RouteCollection()
            };

            // Act & Assert
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    result.ExecuteResult(ControllerContextTest.CreateEmptyContext());
                },
                "No route in the route table matches the supplied values.");
        }

        [TestMethod]
        public void ExecuteResultWithNullControllerContextThrows() {
            // Arrange
            var result = new RedirectToRouteResult(null);

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    result.ExecuteResult(null /* context */);
                },
                "context");
        }

        [TestMethod]
        public void RoutesPropertyDefaultsToGlobalRouteTable() {
            // Act
            var result = new RedirectToRouteResult(new RouteValueDictionary());

            // Assert
            Assert.AreSame(RouteTable.Routes, result.Routes);
        }

        [TestMethod]
        public void RedirectInChildActionThrows() {
            // Arrange
            RouteData routeData = new RouteData();
            routeData.DataTokens[ControllerContext.PARENT_ACTION_VIEWCONTEXT] = new ViewContext();
            ControllerContext context = new ControllerContext(new Mock<HttpContextBase>().Object, routeData, new Mock<ControllerBase>().Object);
            RedirectToRouteResult result = new RedirectToRouteResult(new RouteValueDictionary());

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => result.ExecuteResult(context),
                MvcResources.RedirectAction_CannotRedirectInChildAction
            );
        }

    }
}
