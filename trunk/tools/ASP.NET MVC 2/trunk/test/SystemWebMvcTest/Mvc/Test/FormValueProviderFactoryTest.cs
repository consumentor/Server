namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class FormValueProviderFactoryTest {

        private static readonly NameValueCollection _backingStore = new NameValueCollection() {
            { "foo", "fooValue" }
        };

        [TestMethod]
        public void GetValueProvider() {
            // Arrange
            FormValueProviderFactory factory = new FormValueProviderFactory();

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(o => o.HttpContext.Request.Form).Returns(_backingStore);

            // Act
            IValueProvider valueProvider = factory.GetValueProvider(mockControllerContext.Object);

            // Assert
            Assert.AreEqual(typeof(FormValueProvider), valueProvider.GetType());
            ValueProviderResult vpResult = valueProvider.GetValue("foo");

            Assert.IsNotNull(vpResult, "Should have contained a value for key 'foo'.");
            Assert.AreEqual("fooValue", vpResult.AttemptedValue);
            Assert.AreEqual(CultureInfo.CurrentCulture, vpResult.Culture);
        }

        [TestMethod]
        public void GetValueProvider_ThrowsIfControllerContextIsNull() {
            // Arrange
            FormValueProviderFactory factory = new FormValueProviderFactory();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    factory.GetValueProvider(null);
                }, "controllerContext");
        }

    }
}
