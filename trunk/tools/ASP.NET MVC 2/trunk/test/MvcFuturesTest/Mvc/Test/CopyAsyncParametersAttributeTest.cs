namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class CopyAsyncParametersAttributeTest {

        [TestMethod]
        public void OnActionExecuting_CopiesParametersIfControllerIsAsync() {
            // Arrange
            CopyAsyncParametersAttribute attr = new CopyAsyncParametersAttribute();
            SampleAsyncController controller = new SampleAsyncController();

            ActionExecutingContext filterContext = new ActionExecutingContext() {
                ActionParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase),
                Controller = controller
            };
            filterContext.ActionParameters["foo"] = "fooAction";
            filterContext.ActionParameters["bar"] = "barAction";
            controller.AsyncManager.Parameters["bar"] = "barAsync";
            controller.AsyncManager.Parameters["baz"] = "bazAsync";

            // Act
            attr.OnActionExecuting(filterContext);

            // Assert
            Assert.AreEqual("fooAction", controller.AsyncManager.Parameters["foo"]);
            Assert.AreEqual("barAction", controller.AsyncManager.Parameters["bar"]);
            Assert.AreEqual("bazAsync", controller.AsyncManager.Parameters["baz"]);
        }

        [TestMethod]
        public void OnActionExecuting_DoesNothingIfControllerNotAsync() {
            // Arrange
            CopyAsyncParametersAttribute attr = new CopyAsyncParametersAttribute();
            SampleSyncController controller = new SampleSyncController();

            ActionExecutingContext filterContext = new ActionExecutingContext() {
                ActionParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase),
                Controller = controller
            };
            filterContext.ActionParameters["foo"] = "originalFoo";
            filterContext.ActionParameters["bar"] = "originalBar";

            // Act
            attr.OnActionExecuting(filterContext);

            // Assert
            // If we got this far without crashing, life is good :)
        }

        [TestMethod]
        public void OnActionExecuting_ThrowsIfFilterContextIsNull() {
            // Arrange
            CopyAsyncParametersAttribute attr = new CopyAsyncParametersAttribute();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    attr.OnActionExecuting(null);
                }, "filterContext");
        }

        private class SampleSyncController : Controller {
        }

        private class SampleAsyncController : AsyncController {
        }

    }
}
