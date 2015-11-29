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
    public class MvcDynamicSessionControllerFactoryTest {

        [TestMethod]
        public void Constructor_ThrowsIfOriginalFactoryIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new MvcDynamicSessionControllerFactory(null);
                }, "originalFactory");
        }

        [TestMethod]
        public void CreateCachedController_UnderlyingFactoryReturnsController() {
            // Arrange
            RequestContext requestContext = GetRequestContext();
            EmptyController controller = new EmptyController();

            Mock<IControllerFactory> mockUnderlyingFactory = new Mock<IControllerFactory>();
            mockUnderlyingFactory.Expect(o => o.CreateController(requestContext, "home")).Returns(controller).AtMostOnce();
            MvcDynamicSessionControllerFactory factory = new MvcDynamicSessionControllerFactory(mockUnderlyingFactory.Object);

            // Act
            IController controller1 = factory.CreateCachedController(requestContext, "home");
            IController controller2 = factory.CreateController(requestContext, "home");

            // Assert
            Assert.AreEqual(controller, controller1);
            Assert.AreSame(controller1, controller2);
            mockUnderlyingFactory.Verify();
        }

        [TestMethod]
        public void CreateCachedController_UnderlyingFactoryReturnsNull() {
            // Arrange
            RequestContext requestContext = GetRequestContext();

            Mock<IControllerFactory> mockUnderlyingFactory = new Mock<IControllerFactory>();
            mockUnderlyingFactory.Expect(o => o.CreateController(requestContext, "home")).Returns((IController)null).AtMostOnce();
            MvcDynamicSessionControllerFactory factory = new MvcDynamicSessionControllerFactory(mockUnderlyingFactory.Object);

            // Act
            IController controller1 = factory.CreateCachedController(requestContext, "home");
            IController controller2 = factory.CreateController(requestContext, "home");

            // Assert
            Assert.IsNull(controller1);
            Assert.IsNull(controller2);
            mockUnderlyingFactory.Verify();
        }

        [TestMethod]
        public void CreateController_RemovesCachedController() {
            // Arrange
            RequestContext requestContext = GetRequestContext();
            EmptyController controller = new EmptyController();
            int numTimesCalled = 0;

            Mock<IControllerFactory> mockUnderlyingFactory = new Mock<IControllerFactory>();
            mockUnderlyingFactory.Expect(o => o.CreateController(requestContext, "home")).Returns(controller).Callback(() => { numTimesCalled++; });
            MvcDynamicSessionControllerFactory factory = new MvcDynamicSessionControllerFactory(mockUnderlyingFactory.Object);

            // Act & assert 1
            IController controller1 = factory.CreateCachedController(requestContext, "home");
            IController controller2 = factory.CreateController(requestContext, "home");
            Assert.AreEqual(1, numTimesCalled);

            // Act & assert 2
            IController controller3 = factory.CreateController(requestContext, "home");
            Assert.AreEqual(2, numTimesCalled);
        }

        [TestMethod]
        public void CreateController_ThrowsIfRequestContextIsNull() {
            // Arrange
            MvcDynamicSessionControllerFactory factory = new MvcDynamicSessionControllerFactory();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    factory.CreateController(null, "home");
                }, "requestContext");
        }

        [TestMethod]
        public void ReleaseCachedController_ControllerInstanceCached_ReleasesInstance() {
            // Arrange
            RequestContext requestContext = GetRequestContext();
            EmptyController controller = new EmptyController();

            Mock<IControllerFactory> mockUnderlyingFactory = new Mock<IControllerFactory>();
            mockUnderlyingFactory.Expect(o => o.CreateController(requestContext, "home")).Returns(controller).AtMostOnce();
            mockUnderlyingFactory.Expect(o => o.ReleaseController(controller)).Verifiable();
            MvcDynamicSessionControllerFactory factory = new MvcDynamicSessionControllerFactory(mockUnderlyingFactory.Object);

            // Act
            factory.CreateCachedController(requestContext, "home");
            factory.ReleaseCachedController(requestContext.HttpContext);

            // Assert
            mockUnderlyingFactory.Verify();
        }

        [TestMethod]
        public void ReleaseCachedController_ControllerInstanceNotCached_DoesNothing() {
            // Arrange
            RequestContext requestContext = GetRequestContext();

            Mock<IControllerFactory> mockUnderlyingFactory = new Mock<IControllerFactory>();
            mockUnderlyingFactory.Expect(o => o.ReleaseController(It.IsAny<IController>())).Never();
            MvcDynamicSessionControllerFactory factory = new MvcDynamicSessionControllerFactory(mockUnderlyingFactory.Object);

            // Act
            factory.ReleaseCachedController(requestContext.HttpContext);

            // Assert
            // If we got this far, success!
        }

        [TestMethod]
        public void ReleaseController_ForwardsToUnderlyingFactory() {
            // Arrange
            EmptyController controller = new EmptyController();

            Mock<IControllerFactory> mockUnderlyingFactory = new Mock<IControllerFactory>();
            mockUnderlyingFactory.Expect(o => o.ReleaseController(controller)).Verifiable();
            MvcDynamicSessionControllerFactory factory = new MvcDynamicSessionControllerFactory(mockUnderlyingFactory.Object);

            // Act
            factory.ReleaseController(controller);

            // Assert
            mockUnderlyingFactory.Verify();
        }

        private static RequestContext GetRequestContext() {
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(o => o.Items).Returns(new Hashtable());

            return new RequestContext(mockHttpContext.Object, new RouteData());
        }

        private class EmptyController : Controller {
        }

    }
}
