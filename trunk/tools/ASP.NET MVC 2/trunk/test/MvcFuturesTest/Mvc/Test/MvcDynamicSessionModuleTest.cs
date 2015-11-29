namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Collections;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class MvcDynamicSessionModuleTest {

        [TestMethod]
        public void Dispose_DoesNothing() {
            // Arrange
            MvcDynamicSessionModule module = new MvcDynamicSessionModule();

            // Act
            module.Dispose();

            // Assert
            // No exception - life is good
        }

        [TestMethod]
        public void PossiblyReleaseController_CorrectFactory_ReleasesController() {
            // Arrange
            RequestContext requestContext = GetRequestContext();
            IHttpHandler originalHttpHandler = requestContext.HttpContext.Handler;
            Controller controller = new ControllerReadOnlySession();

            Mock<IControllerFactory> mockControllerFactory = new Mock<IControllerFactory>();
            mockControllerFactory.Expect(o => o.CreateController(requestContext, "home")).Returns(controller);
            mockControllerFactory.Expect(o => o.ReleaseController(controller)).Verifiable();

            ControllerBuilder controllerBuilder = new ControllerBuilder();
            controllerBuilder.SetControllerFactory(new MvcDynamicSessionControllerFactory(mockControllerFactory.Object));
            MvcDynamicSessionModule module = new MvcDynamicSessionModule() {
                ControllerBuilder = controllerBuilder
            };

            // Act
            module.SetSessionStateMode(requestContext.HttpContext, SimpleDynamicSessionStateConfigurator.ExpectMode(ControllerSessionState.ReadOnly));
            MvcDynamicSessionModule.PossiblyReleaseController(requestContext.HttpContext);

            // Assert
            mockControllerFactory.Verify();
        }

        [TestMethod]
        public void SetSessionStateMode_HandlerIsMvcHandler_ControllerHasAttribute_SetsAttributeValue() {
            // Arrange
            RequestContext requestContext = GetRequestContext();
            IHttpHandler originalHttpHandler = requestContext.HttpContext.Handler;

            Mock<IControllerFactory> mockControllerFactory = new Mock<IControllerFactory>();
            mockControllerFactory.Expect(o => o.CreateController(requestContext, "home")).Returns(new ControllerReadOnlySession());

            ControllerBuilder controllerBuilder = new ControllerBuilder();
            controllerBuilder.SetControllerFactory(new MvcDynamicSessionControllerFactory(mockControllerFactory.Object));
            MvcDynamicSessionModule module = new MvcDynamicSessionModule() {
                ControllerBuilder = controllerBuilder
            };

            // Act
            module.SetSessionStateMode(requestContext.HttpContext, SimpleDynamicSessionStateConfigurator.ExpectMode(ControllerSessionState.ReadOnly));

            // Assert
            Assert.AreEqual(2, requestContext.HttpContext.Items.Count, "Cache + factory weren't properly added to Items.");
        }

        [TestMethod]
        public void SetSessionStateMode_HandlerIsMvcHandler_ControllerHasNoAttribute_SetsDefault() {
            // Arrange
            RequestContext requestContext = GetRequestContext();
            IHttpHandler originalHttpHandler = requestContext.HttpContext.Handler;

            Mock<IControllerFactory> mockControllerFactory = new Mock<IControllerFactory>();
            mockControllerFactory.Expect(o => o.CreateController(requestContext, "home")).Returns(new ControllerWithoutAttribute());

            ControllerBuilder controllerBuilder = new ControllerBuilder();
            controllerBuilder.SetControllerFactory(new MvcDynamicSessionControllerFactory(mockControllerFactory.Object));
            MvcDynamicSessionModule module = new MvcDynamicSessionModule() {
                ControllerBuilder = controllerBuilder
            };

            // Act
            module.SetSessionStateMode(requestContext.HttpContext, SimpleDynamicSessionStateConfigurator.ExpectMode(ControllerSessionState.Default));

            // Assert
            Assert.AreEqual(2, requestContext.HttpContext.Items.Count, "Cache + factory weren't properly added to Items.");
        }

        [TestMethod]
        public void SetSessionStateMode_HandlerIsMvcHandler_NullController_SetsDefault() {
            // Arrange
            RequestContext requestContext = GetRequestContext();
            IHttpHandler originalHttpHandler = requestContext.HttpContext.Handler;

            Mock<IControllerFactory> mockControllerFactory = new Mock<IControllerFactory>();
            mockControllerFactory.Expect(o => o.CreateController(requestContext, "home")).Returns((IController)null);

            ControllerBuilder controllerBuilder = new ControllerBuilder();
            controllerBuilder.SetControllerFactory(new MvcDynamicSessionControllerFactory(mockControllerFactory.Object));
            MvcDynamicSessionModule module = new MvcDynamicSessionModule() {
                ControllerBuilder = controllerBuilder
            };

            // Act
            module.SetSessionStateMode(requestContext.HttpContext, SimpleDynamicSessionStateConfigurator.ExpectMode(ControllerSessionState.Default));

            // Assert
            Assert.AreEqual(2, requestContext.HttpContext.Items.Count, "Cache + factory weren't properly added to Items.");
        }

        [TestMethod]
        public void SetSessionStateMode_HandlerIsNotMvcHandler_DoesNothing() {
            // Arrange
            IHttpHandler expectedHandler = new Mock<IHttpHandler>().Object;

            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(o => o.Handler).Returns(expectedHandler);
            mockHttpContext.Expect(o => o.Items).Never();

            MvcDynamicSessionModule module = new MvcDynamicSessionModule();

            // Act
            module.SetSessionStateMode(mockHttpContext.Object, SimpleDynamicSessionStateConfigurator.ExpectNever());

            // Assert
            mockHttpContext.Verify();
        }

        [TestMethod]
        public void SetSessionStateMode_ThrowsIfControllerFactoryIsIncorrect() {
            // Arrange
            RequestContext requestContext = GetRequestContext();
            MvcDynamicSessionModule module = new MvcDynamicSessionModule();

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    module.SetSessionStateMode(requestContext.HttpContext, null);
                },
                @"The ControllerBuilder must return an IControllerFactory of type Microsoft.Web.Mvc.MvcDynamicSessionControllerFactory if the MvcDynamicSessionModule is enabled.");
        }

        private static RequestContext GetRequestContext() {
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();

            RouteData routeData = new RouteData();
            routeData.Values["controller"] = "home";
            RequestContext requestContext = new RequestContext(mockHttpContext.Object, routeData);
            IHttpHandler currentHttpHandler = new MvcHandler(requestContext);
            mockHttpContext.Expect(o => o.Handler).Returns(currentHttpHandler);
            mockHttpContext.Expect(o => o.Items).Returns(new Hashtable());

            return requestContext;
        }

        private class ControllerWithoutAttribute : Controller {
        }

        [ControllerSessionState(ControllerSessionState.ReadOnly)]
        private class ControllerReadOnlySession : Controller {
        }

        private class SimpleDynamicSessionStateConfigurator : IDynamicSessionStateConfigurator {
            private readonly ControllerSessionState? _expectedMode;

            private SimpleDynamicSessionStateConfigurator(ControllerSessionState? expectedMode) {
                _expectedMode = expectedMode;
            }

            public void ConfigureSessionState(ControllerSessionState mode) {
                if (!_expectedMode.HasValue) {
                    Assert.Fail("ConfigureSessionState shouldn't have been called.");
                }
                Assert.AreEqual(_expectedMode, mode, "ConfigureSessionState called with wrong mode.");
            }

            public static SimpleDynamicSessionStateConfigurator ExpectMode(ControllerSessionState mode) {
                return new SimpleDynamicSessionStateConfigurator(mode);
            }

            public static SimpleDynamicSessionStateConfigurator ExpectNever() {
                return new SimpleDynamicSessionStateConfigurator(null);
            }
        }

    }
}
