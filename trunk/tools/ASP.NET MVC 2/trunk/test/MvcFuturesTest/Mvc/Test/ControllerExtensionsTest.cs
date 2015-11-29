namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class ControllerExtensionsTest {
        private const string AppPathModifier = MvcHelper.AppPathModifier;

        [TestMethod]
        public void RedirectToAction_DifferentController() {
            // Act
            RedirectToRouteResult result = ControllerExtensions.RedirectToAction<DifferentController>(new SampleController(), x => x.SomeOtherMethod(84));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("", result.RouteName);
            Assert.AreEqual(3, result.RouteValues.Count);
            Assert.AreEqual("Different", result.RouteValues["controller"]);
            Assert.AreEqual("SomeOtherMethod", result.RouteValues["action"]);
            Assert.AreEqual(84, result.RouteValues["someOtherParameter"]);
        }

        [TestMethod]
        public void RedirectToAction_SameController() {
            // Act
            RedirectToRouteResult result = ControllerExtensions.RedirectToAction<SampleController>(new SampleController(), x => x.SomeMethod(42));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("", result.RouteName);
            Assert.AreEqual(3, result.RouteValues.Count);
            Assert.AreEqual("Sample", result.RouteValues["controller"]);
            Assert.AreEqual("SomeMethod", result.RouteValues["action"]);
            Assert.AreEqual(42, result.RouteValues["someParameter"]);
        }

        [TestMethod]
        public void RedirectToAction_ThrowsIfControllerIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    ControllerExtensions.RedirectToAction<SampleController>((SampleController)null, x => x.SomeMethod(42));
                }, "controller");
        }

        private class SampleController : Controller {
            public ActionResult SomeMethod(int someParameter) {
                throw new NotImplementedException();
            }
        }

        private class DifferentController : Controller {
            public ActionResult SomeOtherMethod(int someOtherParameter) {
                throw new NotImplementedException();
            }
        }

    }
}
