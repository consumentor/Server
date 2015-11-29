namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Globalization;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ElementalValueProviderTest {

        [TestMethod]
        public void ContainsPrefix() {
            // Arrange
            ElementalValueProvider valueProvider = new ElementalValueProvider("foo", 42, null);

            // Act & assert
            Assert.IsTrue(valueProvider.ContainsPrefix("foo"));
            Assert.IsFalse(valueProvider.ContainsPrefix("bar"));
        }

        [TestMethod]
        public void GetValue_NameDoesNotMatch_ReturnsNull() {
            // Arrange
            CultureInfo culture = CultureInfo.GetCultureInfo("fr-FR");
            DateTime rawValue = new DateTime(2001, 1, 2);
            ElementalValueProvider valueProvider = new ElementalValueProvider("foo", rawValue, culture);

            // Act
            ValueProviderResult vpResult = valueProvider.GetValue("bar");

            // Assert
            Assert.IsNull(vpResult);
        }

        [TestMethod]
        public void GetValue_NameMatches_ReturnsValueProviderResult() {
            // Arrange
            CultureInfo culture = CultureInfo.GetCultureInfo("fr-FR");
            DateTime rawValue = new DateTime(2001, 1, 2);
            ElementalValueProvider valueProvider = new ElementalValueProvider("foo", rawValue, culture);

            // Act
            ValueProviderResult vpResult = valueProvider.GetValue("FOO");

            // Assert
            Assert.IsNotNull(vpResult);
            Assert.AreEqual(rawValue, vpResult.RawValue);
            Assert.AreEqual("02/01/2001 00:00:00", vpResult.AttemptedValue);
            Assert.AreEqual(culture, vpResult.Culture);
        }

    }
}
