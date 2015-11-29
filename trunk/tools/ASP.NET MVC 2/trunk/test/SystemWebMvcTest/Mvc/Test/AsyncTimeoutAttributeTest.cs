namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AsyncTimeoutAttributeTest {

        [TestMethod]
        public void ConstructorThrowsIfDurationIsOutOfRange() {
            // Act & assert
            ExceptionHelper.ExpectArgumentOutOfRangeException(
                delegate {
                    new AsyncTimeoutAttribute(-1000);
                }, "duration",
                @"The timeout value must be non-negative or Timeout.Infinite.
Parameter name: duration");
        }

        [TestMethod]
        public void DurationProperty() {
            // Act
            AsyncTimeoutAttribute attr = new AsyncTimeoutAttribute(45);

            // Assert
            Assert.AreEqual(45, attr.Duration);
        }

        [TestMethod]
        public void OnActionExecutingSetsTimeoutPropertyOnController() {
            // Arrange
            AsyncTimeoutAttribute attr = new AsyncTimeoutAttribute(45);

            MyAsyncController controller = new MyAsyncController();
            controller.AsyncManager.Timeout = 0;

            ActionExecutingContext filterContext = new ActionExecutingContext() {
                Controller = controller
            };

            // Act
            attr.OnActionExecuting(filterContext);

            // Assert
            Assert.AreEqual(45, controller.AsyncManager.Timeout);
        }

        [TestMethod]
        public void OnActionExecutingThrowsIfControllerIsNotAsyncManagerContainer() {
            // Arrange
            AsyncTimeoutAttribute attr = new AsyncTimeoutAttribute(45);

            ActionExecutingContext filterContext = new ActionExecutingContext() {
                Controller = new MyController()
            };

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    attr.OnActionExecuting(filterContext);
                },
                @"The controller of type 'System.Web.Mvc.Test.AsyncTimeoutAttributeTest+MyController' must subclass AsyncController or implement the IAsyncManagerContainer interface.");
        }

        [TestMethod]
        public void OnActionExecutingThrowsIfFilterContextIsNull() {
            // Arrange
            AsyncTimeoutAttribute attr = new AsyncTimeoutAttribute(45);

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    attr.OnActionExecuting(null);
                }, "filterContext");
        }

        private class MyController : Controller {
        }

        private class MyAsyncController : AsyncController {
        }

    }
}
