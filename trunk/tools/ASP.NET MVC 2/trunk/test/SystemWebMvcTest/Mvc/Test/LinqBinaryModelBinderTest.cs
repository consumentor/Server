namespace System.Web.Mvc.Test {
    using System;
    using System.Data.Linq;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class LinqBinaryModelBinderTest {
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

            LinqBinaryModelBinder binder = new LinqBinaryModelBinder();

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

            LinqBinaryModelBinder binder = new LinqBinaryModelBinder();

            // Act
            object binderResult = binder.BindModel(null, bindingContext);

            // Assert
            Assert.IsNull(binderResult);
        }

        [TestMethod]
        public void BindModelThrowsIfBindingContextIsNull() {
            // Arrange
            LinqBinaryModelBinder binder = new LinqBinaryModelBinder();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    binder.BindModel(null, null);
                }, "bindingContext");
        }

        [TestMethod]
        public void BindModelWithBase64QuotedValueReturnsBinary() {
            // Arrange
            string base64Value = ByteArrayModelBinderTest.Base64TestString;

            SimpleValueProvider valueProvider = new SimpleValueProvider() {
                { "foo", "\"" + base64Value + "\"" }
            };

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ValueProvider = valueProvider
            };

            LinqBinaryModelBinder binder = new LinqBinaryModelBinder();

            // Act
            Binary boundValue = binder.BindModel(null, bindingContext) as Binary;

            // Assert
            Assert.AreEqual(ByteArrayModelBinderTest.Base64TestBytes, boundValue);
        }

        [TestMethod]
        public void BindModelWithBase64UnquotedValueReturnsBinary() {
            // Arrange
            string base64Value = ByteArrayModelBinderTest.Base64TestString;
            SimpleValueProvider valueProvider = new SimpleValueProvider() {
                { "foo", base64Value }
            };

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelName = "foo",
                ValueProvider = valueProvider
            };

            LinqBinaryModelBinder binder = new LinqBinaryModelBinder();

            // Act
            Binary boundValue = binder.BindModel(null, bindingContext) as Binary;

            // Assert
            Assert.AreEqual(ByteArrayModelBinderTest.Base64TestBytes, boundValue);
        }
    }
}
