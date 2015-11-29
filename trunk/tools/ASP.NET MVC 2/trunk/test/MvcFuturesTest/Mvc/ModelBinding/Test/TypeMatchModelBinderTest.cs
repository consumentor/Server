namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class TypeMatchModelBinderTest {

        [TestMethod]
        public void BindModel_InvalidValueProviderResult_ReturnsFalse() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = GetBindingContext();
            bindingContext.ValueProvider = new SimpleValueProvider() {
                { "theModelName", "not an integer" }
            };

            TypeMatchModelBinder binder = new TypeMatchModelBinder();

            // Act
            bool retVal = binder.BindModel(null, bindingContext);

            // Assert
            Assert.IsFalse(retVal);
            Assert.AreEqual(0, bindingContext.ModelState.Count, "ModelState shouldn't have been touched.");
        }

        [TestMethod]
        public void BindModel_ValidValueProviderResult_ReturnsTrue() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = GetBindingContext();
            bindingContext.ValueProvider = new SimpleValueProvider() {
                { "theModelName", 42 }
            };

            TypeMatchModelBinder binder = new TypeMatchModelBinder();

            // Act
            bool retVal = binder.BindModel(null, bindingContext);

            // Assert
            Assert.IsTrue(retVal);
            Assert.AreEqual(42, bindingContext.Model);
            Assert.IsTrue(bindingContext.ModelState.ContainsKey("theModelName"), "ModelState should've been updated.");
        }

        [TestMethod]
        public void GetCompatibleValueProviderResult_ValueProviderResultRawValueIncorrect_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = GetBindingContext();
            bindingContext.ValueProvider = new SimpleValueProvider() {
                { "theModelName", "not an integer" }
            };

            // Act
            ValueProviderResult vpResult = TypeMatchModelBinder.GetCompatibleValueProviderResult(bindingContext);

            // Assert
            Assert.IsNull(vpResult, "Should have been null because the RawValue is of the wrong type.");
        }

        [TestMethod]
        public void GetCompatibleValueProviderResult_ValueProviderResultValid_ReturnsValueProviderResult() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = GetBindingContext();
            bindingContext.ValueProvider = new SimpleValueProvider() {
                { "theModelName", 42 }
            };

            // Act
            ValueProviderResult vpResult = TypeMatchModelBinder.GetCompatibleValueProviderResult(bindingContext);

            // Assert
            Assert.IsNotNull(vpResult);
        }

        [TestMethod]
        public void GetCompatibleValueProviderResult_ValueProviderReturnsNull_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = GetBindingContext();
            bindingContext.ValueProvider = new SimpleValueProvider();

            // Act
            ValueProviderResult vpResult = TypeMatchModelBinder.GetCompatibleValueProviderResult(bindingContext);

            // Assert
            Assert.IsNull(vpResult, "Should have been null because no key matched.");
        }

        private static ExtensibleModelBindingContext GetBindingContext() {
            return GetBindingContext(typeof(int));
        }

        private static ExtensibleModelBindingContext GetBindingContext(Type modelType) {
            return new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, modelType),
                ModelName = "theModelName"
            };
        }

    }
}
