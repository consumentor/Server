namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DictionaryValueProviderTest {

        private static readonly Dictionary<string, object> _backingStore = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) {
            { "forty.two", 42 },
            { "nineteen.eighty.four", new DateTime(1984, 1, 1) }
        };

        [TestMethod]
        public void Constructor_ThrowsIfDictionaryIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new DictionaryValueProvider<object>(null, CultureInfo.InvariantCulture);
                }, "dictionary");
        }

        [TestMethod]
        public void ContainsPrefix() {
            // Arrange
            DictionaryValueProvider<object> valueProvider = new DictionaryValueProvider<object>(_backingStore, null);

            // Act
            bool result = valueProvider.ContainsPrefix("forty");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsPrefix_DoesNotContainEmptyPrefixIfBackingStoreIsEmpty() {
            // Arrange
            DictionaryValueProvider<object> valueProvider = new DictionaryValueProvider<object>(new Dictionary<string, object>(), null);

            // Act
            bool result = valueProvider.ContainsPrefix("");

            // Assert
            Assert.IsFalse(result, "The '' prefix shouldn't have been present.");
        }

        [TestMethod]
        public void ContainsPrefix_ThrowsIfPrefixIsNull() {
            // Arrange
            DictionaryValueProvider<object> valueProvider = new DictionaryValueProvider<object>(_backingStore, null);

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    valueProvider.ContainsPrefix(null);
                }, "prefix");
        }

        [TestMethod]
        public void GetValue() {
            // Arrange
            CultureInfo culture = CultureInfo.GetCultureInfo("fr-FR");
            DictionaryValueProvider<object> valueProvider = new DictionaryValueProvider<object>(_backingStore, culture);

            // Act
            ValueProviderResult vpResult = valueProvider.GetValue("nineteen.eighty.four");

            // Assert
            Assert.IsNotNull(vpResult);
            Assert.AreEqual(new DateTime(1984, 1, 1), vpResult.RawValue);
            Assert.AreEqual("01/01/1984 00:00:00", vpResult.AttemptedValue);
            Assert.AreEqual(culture, vpResult.Culture);
        }

        [TestMethod]
        public void GetValue_ReturnsNullIfKeyNotFound() {
            // Arrange
            DictionaryValueProvider<object> valueProvider = new DictionaryValueProvider<object>(_backingStore, null);

            // Act
            ValueProviderResult vpResult = valueProvider.GetValue("nineteen.eighty");

            // Assert
            Assert.IsNull(vpResult);
        }

        [TestMethod]
        public void GetValue_ThrowsIfKeyIsNull() {
            // Arrange
            DictionaryValueProvider<object> valueProvider = new DictionaryValueProvider<object>(_backingStore, null);

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    valueProvider.GetValue(null);
                }, "key");
        }

    }
}
