namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Web;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class DynamicSessionStateConfigurator35Test {

        [TestMethod]
        public void ConfigureSessionState_Default_DoesNothing() {
            // Arrange
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.ExpectSet(o => o.Handler).Never();
            HttpContextBase httpContext = mockHttpContext.Object;

            DynamicSessionStateConfigurator35 configurator = new DynamicSessionStateConfigurator35(httpContext);

            // Act
            configurator.ConfigureSessionState(ControllerSessionState.Default);
            IHttpHandler newHandler = httpContext.Handler;

            // Assert
            mockHttpContext.Verify();
        }

        [TestMethod]
        public void ConfigureSessionState_Disabled_ChangesHandler() {
            // Arrange
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.SetupProperty(o => o.Handler);
            HttpContextBase httpContext = mockHttpContext.Object;

            DynamicSessionStateConfigurator35 configurator = new DynamicSessionStateConfigurator35(httpContext);

            // Act
            configurator.ConfigureSessionState(ControllerSessionState.Disabled);
            IHttpHandler newHandler = httpContext.Handler;

            // Assert
            Assert.AreEqual(typeof(MvcDynamicSessionHandler), newHandler.GetType());
        }

        [TestMethod]
        public void ConfigureSessionState_ReadOnly_ChangesHandler() {
            // Arrange
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.SetupProperty(o => o.Handler);
            HttpContextBase httpContext = mockHttpContext.Object;

            DynamicSessionStateConfigurator35 configurator = new DynamicSessionStateConfigurator35(httpContext);

            // Act
            configurator.ConfigureSessionState(ControllerSessionState.ReadOnly);
            IHttpHandler newHandler = httpContext.Handler;

            // Assert
            Assert.AreEqual(typeof(MvcReadOnlySessionHandler), newHandler.GetType());
        }

    }
}
