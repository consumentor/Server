namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AuthorizationContextTest {

        [TestMethod]
        public void ConstructorThrowsIfActionDescriptorIsNull() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            ActionDescriptor actionDescriptor = null;

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new AuthorizationContext(controllerContext, actionDescriptor);
                }, "actionDescriptor");
        }

        [TestMethod]
        public void PropertiesAreSetByConstructor() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            ActionDescriptor actionDescriptor = new Mock<ActionDescriptor>().Object;

            // Act
            AuthorizationContext authorizationContext = new AuthorizationContext(controllerContext, actionDescriptor);

            // Assert
            Assert.AreEqual(actionDescriptor, authorizationContext.ActionDescriptor);
        }

    }
}
