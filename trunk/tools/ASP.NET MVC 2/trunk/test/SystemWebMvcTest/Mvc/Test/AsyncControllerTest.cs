namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Async;
    using System.Web.Mvc.Async.Test;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AsyncControllerTest {

        [TestMethod]
        public void ActionInvokerProperty() {
            // Arrange
            EmptyController controller = new EmptyController();

            // Act
            IActionInvoker invoker = controller.ActionInvoker;

            // Assert
            Assert.AreEqual(typeof(AsyncControllerActionInvoker), invoker.GetType());
        }

        [TestMethod]
        public void AsyncManagerProperty() {
            // Arrange
            EmptyController controller = new EmptyController();

            // Act
            AsyncManager asyncManager = controller.AsyncManager;

            // Assert
            Assert.IsNotNull(asyncManager);
        }

        [TestMethod]
        public void Execute_ThrowsIfCalledMoreThanOnce() {
            // Arrange
            IAsyncController controller = new EmptyController();
            RequestContext requestContext = GetRequestContext("SomeAction");

            // Act & assert
            controller.BeginExecute(requestContext, null, null);
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    controller.BeginExecute(requestContext, null, null);
                },
                @"A single instance of controller 'System.Web.Mvc.Test.AsyncControllerTest+EmptyController' cannot be used to handle multiple requests. If a custom controller factory is in use, make sure that it creates a new instance of the controller for each request.");
        }

        [TestMethod]
        public void Execute_ThrowsIfRequestContextIsNull() {
            // Arrange
            IAsyncController controller = new EmptyController();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    controller.BeginExecute(null, null, null);
                }, "requestContext");
        }

        [TestMethod]
        public void ExecuteCore_Asynchronous_ActionFound() {
            // Arrange
            MockAsyncResult innerAsyncResult = new MockAsyncResult();

            Mock<IAsyncActionInvoker> mockActionInvoker = new Mock<IAsyncActionInvoker>();
            mockActionInvoker.Expect(o => o.BeginInvokeAction(It.IsAny<ControllerContext>(), "SomeAction", It.IsAny<AsyncCallback>(), It.IsAny<object>())).Returns(innerAsyncResult);
            mockActionInvoker.Expect(o => o.EndInvokeAction(innerAsyncResult)).Returns(true);

            RequestContext requestContext = GetRequestContext("SomeAction");
            EmptyController controller = new EmptyController() {
                ActionInvoker = mockActionInvoker.Object
            };

            // Act & assert
            IAsyncResult outerAsyncResult = ((IAsyncController)controller).BeginExecute(requestContext, null, null);
            Assert.IsFalse(controller.TempDataSaved, "TempData shouldn't have been saved yet.");

            ((IAsyncController)controller).EndExecute(outerAsyncResult);
            Assert.IsTrue(controller.TempDataSaved);
            Assert.IsFalse(controller.HandleUnknownActionCalled);
        }

        [TestMethod]
        public void ExecuteCore_Asynchronous_ActionNotFound() {
            // Arrange
            MockAsyncResult innerAsyncResult = new MockAsyncResult();

            Mock<IAsyncActionInvoker> mockActionInvoker = new Mock<IAsyncActionInvoker>();
            mockActionInvoker.Expect(o => o.BeginInvokeAction(It.IsAny<ControllerContext>(), "SomeAction", It.IsAny<AsyncCallback>(), It.IsAny<object>())).Returns(innerAsyncResult);
            mockActionInvoker.Expect(o => o.EndInvokeAction(innerAsyncResult)).Returns(false);

            RequestContext requestContext = GetRequestContext("SomeAction");
            EmptyController controller = new EmptyController() {
                ActionInvoker = mockActionInvoker.Object
            };

            // Act & assert
            IAsyncResult outerAsyncResult = ((IAsyncController)controller).BeginExecute(requestContext, null, null);
            Assert.IsFalse(controller.TempDataSaved, "TempData shouldn't have been saved yet.");

            ((IAsyncController)controller).EndExecute(outerAsyncResult);
            Assert.IsTrue(controller.TempDataSaved);
            Assert.IsTrue(controller.HandleUnknownActionCalled);
        }

        [TestMethod]
        public void ExecuteCore_Synchronous_ActionFound() {
            // Arrange
            MockAsyncResult innerAsyncResult = new MockAsyncResult();

            Mock<IActionInvoker> mockActionInvoker = new Mock<IActionInvoker>();
            mockActionInvoker.Expect(o => o.InvokeAction(It.IsAny<ControllerContext>(), "SomeAction")).Returns(true);

            RequestContext requestContext = GetRequestContext("SomeAction");
            EmptyController controller = new EmptyController() {
                ActionInvoker = mockActionInvoker.Object
            };

            // Act & assert
            IAsyncResult outerAsyncResult = ((IAsyncController)controller).BeginExecute(requestContext, null, null);
            Assert.IsFalse(controller.TempDataSaved, "TempData shouldn't have been saved yet.");

            ((IAsyncController)controller).EndExecute(outerAsyncResult);
            Assert.IsTrue(controller.TempDataSaved);
            Assert.IsFalse(controller.HandleUnknownActionCalled);
        }

        [TestMethod]
        public void ExecuteCore_Synchronous_ActionNotFound() {
            // Arrange
            MockAsyncResult innerAsyncResult = new MockAsyncResult();

            Mock<IActionInvoker> mockActionInvoker = new Mock<IActionInvoker>();
            mockActionInvoker.Expect(o => o.InvokeAction(It.IsAny<ControllerContext>(), "SomeAction")).Returns(false);

            RequestContext requestContext = GetRequestContext("SomeAction");
            EmptyController controller = new EmptyController() {
                ActionInvoker = mockActionInvoker.Object
            };

            // Act & assert
            IAsyncResult outerAsyncResult = ((IAsyncController)controller).BeginExecute(requestContext, null, null);
            Assert.IsFalse(controller.TempDataSaved, "TempData shouldn't have been saved yet.");

            ((IAsyncController)controller).EndExecute(outerAsyncResult);
            Assert.IsTrue(controller.TempDataSaved);
            Assert.IsTrue(controller.HandleUnknownActionCalled);
        }

        [TestMethod]
        public void ExecuteCore_SavesTempDataOnException() {
            // Arrange
            Mock<IAsyncActionInvoker> mockActionInvoker = new Mock<IAsyncActionInvoker>();
            mockActionInvoker
                .Expect(o => o.BeginInvokeAction(It.IsAny<ControllerContext>(), "SomeAction", It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                .Throws(new Exception("Some exception text."));

            RequestContext requestContext = GetRequestContext("SomeAction");
            EmptyController controller = new EmptyController() {
                ActionInvoker = mockActionInvoker.Object
            };

            // Act & assert
            ExceptionHelper.ExpectException<Exception>(
                delegate {
                    ((IAsyncController)controller).BeginExecute(requestContext, null, null);
                },
                @"Some exception text.");
            Assert.IsTrue(controller.TempDataSaved);
        }

        private static RequestContext GetRequestContext(string actionName) {
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            RouteData routeData = new RouteData();
            routeData.Values["action"] = actionName;

            return new RequestContext(mockHttpContext.Object, routeData);
        }

        private class EmptyController : AsyncController {
            public bool TempDataSaved;
            public bool HandleUnknownActionCalled;

            protected override ITempDataProvider CreateTempDataProvider() {
                return new DummyTempDataProvider();
            }

            protected override void HandleUnknownAction(string actionName) {
                HandleUnknownActionCalled = true;
            }

            private class DummyTempDataProvider : ITempDataProvider {
                public IDictionary<string, object> LoadTempData(ControllerContext controllerContext) {
                    return new TempDataDictionary();
                }

                public void SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values) {
                    ((EmptyController)controllerContext.Controller).TempDataSaved = true;
                }
            }
        }

    }
}
