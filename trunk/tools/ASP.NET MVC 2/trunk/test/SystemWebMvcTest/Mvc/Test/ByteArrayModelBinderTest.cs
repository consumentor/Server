namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class ByteArrayModelBinderTest {
        internal const string Base64TestString = "Fys1";
        internal static readonly byte[] Base64TestBytes = new byte[] { 23, 43, 53 };

        [TestMethod]
        public void BindModelWithNonExistentValueReturnsNull() {
            // Arrange
            SimpleValueProvider valueProvider = new SimpleValueProvider() {
                { "foo", null }
            };

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ValueProvider = valueProvider
            };

            ByteArrayModelBinder binder = new ByteArrayModelBinder();

            // Act
            object binderResult = binder.BindModel(null, bindingContext);

            // Assert
            Assert.IsNull(binderResult);
        }

        [TestMethod]
        public void BinderWithEmptyStringValueReturnsNull() {
            // Arrange
            SimpleValueProvider valueProvider = new SimpleValueProvider() {
                { "foo", "" }
            };

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ValueProvider = valueProvider
            };

            ByteArrayModelBinder binder = new ByteArrayModelBinder();

            // Act
            object binderResult = binder.BindModel(null, bindingContext);

            // Assert
            Assert.IsNull(binderResult);
        }

        [TestMethod]
        public void BindModelThrowsIfBindingContextIsNull() {
            // Arrange
            ByteArrayModelBinder binder = new ByteArrayModelBinder();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    binder.BindModel(null, null);
                }, "bindingContext");
        }

        [TestMethod]
        public void BindModelWithBase64QuotedValueReturnsByteArray() {
            // Arrange
            string base64Value = Base64TestString;
            SimpleValueProvider valueProvider = new SimpleValueProvider() {
                { "foo", "\"" + base64Value + "\"" }
            };

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ValueProvider = valueProvider
            };

            ByteArrayModelBinder binder = new ByteArrayModelBinder();

            // Act
            byte[] boundValue = binder.BindModel(null, bindingContext) as byte[];

            // Assert
            CollectionAssert.AreEqual(Base64TestBytes, boundValue);
        }

        [TestMethod]
        public void BindModelWithBase64UnquotedValueReturnsByteArray() {
            // Arrange
            string base64Value = Base64TestString;
            SimpleValueProvider valueProvider = new SimpleValueProvider() {
                { "foo", base64Value }
            };

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ValueProvider = valueProvider
            };

            ByteArrayModelBinder binder = new ByteArrayModelBinder();

            // Act
            byte[] boundValue = binder.BindModel(null, bindingContext) as byte[];

            // Assert
            CollectionAssert.AreEqual(Base64TestBytes, boundValue);
        }
    }
}
