namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NameValueCollectionValueProviderTest {

        private static readonly NameValueCollection _backingStore = new NameValueCollection() {
            { "foo", "fooValue1" },
            { "foo", "fooValue2" },
            { "bar.baz", "someOtherValue" }
        };

        [TestMethod]
        public void Constructor_ThrowsIfCollectionIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new NameValueCollectionValueProvider(null, CultureInfo.InvariantCulture);
                }, "collection");
        }

        [TestMethod]
        public void ContainsPrefix() {
            // Arrange
            NameValueCollectionValueProvider valueProvider = new NameValueCollectionValueProvider(_backingStore, null);

            // Act
            bool result = valueProvider.ContainsPrefix("bar");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsPrefix_DoesNotContainEmptyPrefixIfBackingStoreIsEmpty() {
            // Arrange
            NameValueCollectionValueProvider valueProvider = new NameValueCollectionValueProvider(new NameValueCollection(), null);

            // Act
            bool result = valueProvider.ContainsPrefix("");

            // Assert
            Assert.IsFalse(result, "The '' prefix shouldn't have been present.");
        }

        [TestMethod]
        public void ContainsPrefix_ThrowsIfPrefixIsNull() {
            // Arrange
            NameValueCollectionValueProvider valueProvider = new NameValueCollectionValueProvider(_backingStore, null);

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
            NameValueCollectionValueProvider valueProvider = new NameValueCollectionValueProvider(_backingStore, culture);

            // Act
            ValueProviderResult vpResult = valueProvider.GetValue("foo");

            // Assert
            Assert.IsNotNull(vpResult);
            CollectionAssert.AreEqual(_backingStore.GetValues("foo"), (string[])vpResult.RawValue);
            Assert.AreEqual("fooValue1,fooValue2", vpResult.AttemptedValue);
            Assert.AreEqual(culture, vpResult.Culture);
        }

        [TestMethod]
        public void GetValue_ReturnsNullIfKeyNotFound() {
            // Arrange
            NameValueCollectionValueProvider valueProvider = new NameValueCollectionValueProvider(_backingStore, null);

            // Act
            ValueProviderResult vpResult = valueProvider.GetValue("bar");

            // Assert
            Assert.IsNull(vpResult);
        }

        [TestMethod]
        public void GetValue_ThrowsIfKeyIsNull() {
            // Arrange
            NameValueCollectionValueProvider valueProvider = new NameValueCollectionValueProvider(_backingStore, null);

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    valueProvider.GetValue(null);
                }, "key");
        }

    }
}
