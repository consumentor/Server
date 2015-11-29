namespace System.Web.Mvc.Test {
    using System;
    using System.Globalization;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RouteDataValueProviderFactoryTest {

        [TestMethod]
        public void GetValueProvider() {
            // Arrange
            RouteDataValueProviderFactory factory = new RouteDataValueProviderFactory();

            ControllerContext controllerContext = new ControllerContext();
            controllerContext.RouteData = new RouteData();
            controllerContext.RouteData.Values["forty-two"] = 42;

            // Act
            IValueProvider valueProvider = factory.GetValueProvider(controllerContext);

            // Assert
            Assert.AreEqual(typeof(RouteDataValueProvider), valueProvider.GetType());
            ValueProviderResult vpResult = valueProvider.GetValue("forty-two");

            Assert.IsNotNull(vpResult, "Should have contained a value for key 'forty-two'.");
            Assert.AreEqual(42, vpResult.RawValue);
            Assert.AreEqual("42", vpResult.AttemptedValue);
            Assert.AreEqual(CultureInfo.InvariantCulture, vpResult.Culture);
        }

        [TestMethod]
        public void GetValueProvider_ThrowsIfControllerContextIsNull() {
            // Arrange
            RouteDataValueProviderFactory factory = new RouteDataValueProviderFactory();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    factory.GetValueProvider(null);
                }, "controllerContext");
        }

    }
}
