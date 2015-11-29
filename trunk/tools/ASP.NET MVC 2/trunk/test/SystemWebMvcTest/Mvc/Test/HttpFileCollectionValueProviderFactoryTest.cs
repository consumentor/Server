namespace System.Web.Mvc.Test {
    using System;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class HttpFileCollectionValueProviderFactoryTest {

        [TestMethod]
        public void GetValueProvider() {
            // Arrange
            HttpFileCollectionValueProviderFactory factory = new HttpFileCollectionValueProviderFactory();

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(o => o.HttpContext.Request.Files.Count).Returns(0);

            // Act
            IValueProvider valueProvider = factory.GetValueProvider(mockControllerContext.Object);

            // Assert
            Assert.AreEqual(typeof(HttpFileCollectionValueProvider), valueProvider.GetType());
        }

        [TestMethod]
        public void GetValueProvider_ThrowsIfControllerContextIsNull() {
            // Arrange
            HttpFileCollectionValueProviderFactory factory = new HttpFileCollectionValueProviderFactory();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    factory.GetValueProvider(null);
                }, "controllerContext");
        }

    }
}
