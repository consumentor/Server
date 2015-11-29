namespace System.Web.Mvc.Test {
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Async;
    using System.Web.Mvc.Async.Test;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Moq.Protected;

    [TestClass]
    public class MvcHandlerTest {
        [TestMethod]
        public void ConstructorWithNullRequestContextThrows() {
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new MvcHandler(null);
                },
                "requestContext");
        }

        [TestMethod]
        public void ProcessRequestWithRouteWithoutControllerThrows() {
            // Arrange
            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            contextMock.ExpectMvcVersionResponseHeader().Verifiable();
            RouteData rd = new RouteData();
            MvcHandler mvcHandler = new MvcHandler(new RequestContext(contextMock.Object, rd));

            // Act
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    mvcHandler.ProcessRequest(contextMock.Object);
                },
                "The RouteData must contain an item named 'controller' with a non-empty string value.");

            // Assert
            contextMock.Verify();
        }

        [TestMethod]
        public void ProcessRequestAddsServerHeaderCallsExecute() {
            // Arrange
            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            contextMock.ExpectMvcVersionResponseHeader().Verifiable();

            RouteData rd = new RouteData();
            rd.Values.Add("controller", "foo");
            RequestContext requestContext = new RequestContext(contextMock.Object, rd);
            MvcHandler mvcHandler = new MvcHandler(requestContext);

            Mock<ControllerBase> controllerMock = new Mock<ControllerBase>();
            controllerMock.Protected().Expect("Execute", requestContext).Verifiable();

            ControllerBuilder cb = new ControllerBuilder();
            Mock<IControllerFactory> controllerFactoryMock = new Mock<IControllerFactory>();
            controllerFactoryMock.Expect(o => o.CreateController(requestContext, "foo")).Returns(controllerMock.Object);
            controllerFactoryMock.Expect(o => o.ReleaseController(controllerMock.Object));
            cb.SetControllerFactory(controllerFactoryMock.Object);
            mvcHandler.ControllerBuilder = cb;

            // Act
            mvcHandler.ProcessRequest(contextMock.Object);

            // Assert
            contextMock.Verify();
            controllerMock.Verify();
        }

        [TestMethod]
        public void ProcessRequestRemovesOptionalParametersFromRouteValueDictionary() {
            // Arrange
            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            contextMock.ExpectMvcVersionResponseHeader();

            RouteData rd = new RouteData();
            rd.Values.Add("controller", "foo");
            rd.Values.Add("optional", UrlParameter.Optional);
            RequestContext requestContext = new RequestContext(contextMock.Object, rd);
            MvcHandler mvcHandler = new MvcHandler(requestContext);

            Mock<ControllerBase> controllerMock = new Mock<ControllerBase>();
            controllerMock.Protected().Expect("Execute", requestContext).Verifiable();

            ControllerBuilder cb = new ControllerBuilder();
            Mock<IControllerFactory> controllerFactoryMock = new Mock<IControllerFactory>();
            controllerFactoryMock.Expect(o => o.CreateController(requestContext, "foo")).Returns(controllerMock.Object);
            controllerFactoryMock.Expect(o => o.ReleaseController(controllerMock.Object));
            cb.SetControllerFactory(controllerFactoryMock.Object);
            mvcHandler.ControllerBuilder = cb;

            // Act
            mvcHandler.ProcessRequest(contextMock.Object);

            // Assert
            controllerMock.Verify();
            Assert.IsFalse(rd.Values.ContainsKey("optional"), "Optional value should have been removed.");
        }

        [TestMethod]
        public void ProcessRequestWithDisabledServerHeaderOnlyCallsExecute() {
            bool oldResponseHeaderValue = MvcHandler.DisableMvcResponseHeader;
            try {
                // Arrange
                MvcHandler.DisableMvcResponseHeader = true;
                Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();

                RouteData rd = new RouteData();
                rd.Values.Add("controller", "foo");
                RequestContext requestContext = new RequestContext(contextMock.Object, rd);
                MvcHandler mvcHandler = new MvcHandler(requestContext);

                Mock<ControllerBase> controllerMock = new Mock<ControllerBase>();
                controllerMock.Protected().Expect("Execute", requestContext).Verifiable();

                ControllerBuilder cb = new ControllerBuilder();
                Mock<IControllerFactory> controllerFactoryMock = new Mock<IControllerFactory>();
                controllerFactoryMock.Expect(o => o.CreateController(requestContext, "foo")).Returns(controllerMock.Object);
                controllerFactoryMock.Expect(o => o.ReleaseController(controllerMock.Object));
                cb.SetControllerFactory(controllerFactoryMock.Object);
                mvcHandler.ControllerBuilder = cb;

                // Act
                mvcHandler.ProcessRequest(contextMock.Object);

                // Assert
                controllerMock.Verify();
            }
            finally {
                MvcHandler.DisableMvcResponseHeader = oldResponseHeaderValue;
            }
        }

        [TestMethod]
        public void ProcessRequestDisposesControllerIfExecuteDoesNotThrowException() {
            // Arrange
            Mock<ControllerBase> mockController = new Mock<ControllerBase>();
            mockController.Protected().Expect("Execute", ItExpr.IsAny<RequestContext>()).Verifiable();
            mockController.As<IDisposable>().Expect(d => d.Dispose()).AtMostOnce().Verifiable();

            ControllerBuilder builder = new ControllerBuilder();
            builder.SetControllerFactory(new SimpleControllerFactory(mockController.Object));

            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            contextMock.ExpectMvcVersionResponseHeader().Verifiable();
            RequestContext requestContext = new RequestContext(contextMock.Object, new RouteData());
            requestContext.RouteData.Values["controller"] = "fooController";
            MvcHandler handler = new MvcHandler(requestContext) {
                ControllerBuilder = builder
            };

            // Act
            handler.ProcessRequest(requestContext.HttpContext);

            // Assert
            mockController.Verify();
            contextMock.Verify();
        }

        [TestMethod]
        public void ProcessRequestDisposesControllerIfExecuteThrowsException() {
            // Arrange
            Mock<ControllerBase> mockController = new Mock<ControllerBase>(MockBehavior.Strict);
            mockController.Protected().Expect("Execute", ItExpr.IsAny<RequestContext>()).Throws(new Exception("some exception"));
            mockController.As<IDisposable>().Expect(d => d.Dispose()).AtMostOnce().Verifiable();

            ControllerBuilder builder = new ControllerBuilder();
            builder.SetControllerFactory(new SimpleControllerFactory(mockController.Object));

            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            contextMock.ExpectMvcVersionResponseHeader().Verifiable();
            RequestContext requestContext = new RequestContext(contextMock.Object, new RouteData());
            requestContext.RouteData.Values["controller"] = "fooController";
            MvcHandler handler = new MvcHandler(requestContext) {
                ControllerBuilder = builder
            };

            // Act
            ExceptionHelper.ExpectException<Exception>(
                delegate {
                    handler.ProcessRequest(requestContext.HttpContext);
                },
                "some exception");

            // Assert
            mockController.Verify();
            contextMock.Verify();
        }

        [TestMethod]
        public void ProcessRequestAsync_AsyncController_DisposesControllerOnException() {
            // Arrange
            Mock<IAsyncController> mockController = new Mock<IAsyncController>();
            mockController.Expect(o => o.BeginExecute(It.IsAny<RequestContext>(), It.IsAny<AsyncCallback>(), It.IsAny<object>())).Throws(new Exception("Some exception text."));
            mockController.As<IDisposable>().Expect(o => o.Dispose()).Verifiable();

            MvcHandler handler = GetMvcHandler(mockController.Object);

            // Act & assert
            ExceptionHelper.ExpectException<Exception>(
                delegate {
                    handler.BeginProcessRequest(handler.RequestContext.HttpContext, null, null);
                },
                @"Some exception text.");

            mockController.Verify();
        }

        [TestMethod]
        public void ProcessRequestAsync_AsyncController_NormalExecution() {
            // Arrange
            MockAsyncResult innerAsyncResult = new MockAsyncResult();
            bool disposeWasCalled = false;

            Mock<IAsyncController> mockController = new Mock<IAsyncController>();
            mockController.Expect(o => o.BeginExecute(It.IsAny<RequestContext>(), It.IsAny<AsyncCallback>(), It.IsAny<object>())).Returns(innerAsyncResult);
            mockController.Expect(o => o.EndExecute(innerAsyncResult)).AtMostOnce();
            mockController.As<IDisposable>().Expect(o => o.Dispose()).Callback(delegate { disposeWasCalled = true; });

            MvcHandler handler = GetMvcHandler(mockController.Object);

            // Act & assert
            IAsyncResult outerAsyncResult = handler.BeginProcessRequest(handler.RequestContext.HttpContext, null, null);
            Assert.IsFalse(disposeWasCalled, "Dispose shouldn't have been called yet.");

            handler.EndProcessRequest(outerAsyncResult);
            Assert.IsTrue(disposeWasCalled);
        }

        [TestMethod]
        public void ProcessRequestAsync_SyncController_NormalExecution() {
            // Arrange
            bool executeWasCalled = false;
            bool disposeWasCalled = false;

            Mock<IController> mockController = new Mock<IController>();
            mockController.Expect(o => o.Execute(It.IsAny<RequestContext>())).Callback(delegate { executeWasCalled = true; });
            mockController.As<IDisposable>().Expect(o => o.Dispose()).Callback(delegate { disposeWasCalled = true; });

            MvcHandler handler = GetMvcHandler(mockController.Object);

            // Act & assert
            IAsyncResult outerAsyncResult = handler.BeginProcessRequest(handler.RequestContext.HttpContext, null, null);
            Assert.IsFalse(executeWasCalled, "Controller.Execute() shouldn't have been called yet.");
            Assert.IsFalse(disposeWasCalled, "Dispose shouldn't have been called yet.");

            handler.EndProcessRequest(outerAsyncResult);
            Assert.IsTrue(executeWasCalled);
            Assert.IsTrue(disposeWasCalled);
        }

        private static MvcHandler GetMvcHandler(IController controller) {
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(o => o.Response.AddHeader("X-AspNetMvc-Version", "2.0"));

            RouteData routeData = new RouteData();
            routeData.Values["controller"] = "SomeController";
            RequestContext requestContext = new RequestContext(mockHttpContext.Object, routeData);

            ControllerBuilder controllerBuilder = new ControllerBuilder();
            controllerBuilder.SetControllerFactory(new SimpleControllerFactory(controller));

            return new MvcHandler(requestContext) {
                ControllerBuilder = controllerBuilder
            };
        }

        private class SimpleControllerFactory : IControllerFactory {

            private IController _instance;

            public SimpleControllerFactory(IController instance) {
                _instance = instance;
            }

            public IController CreateController(RequestContext context, string controllerName) {
                return _instance;
            }

            public void ReleaseController(IController controller) {
                IDisposable disposable = controller as IDisposable;
                if (disposable != null) {
                    disposable.Dispose();
                }
            }
        }

    }
}
