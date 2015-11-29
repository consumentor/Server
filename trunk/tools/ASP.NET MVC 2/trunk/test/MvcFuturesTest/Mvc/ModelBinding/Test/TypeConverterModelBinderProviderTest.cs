namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class TypeConverterModelBinderProviderTest {

        [TestMethod]
        public void GetBinder_NoTypeConverterExistsFromString_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(void)); // no TypeConverter exists Void -> String

            TypeConverterModelBinderProvider provider = new TypeConverterModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = provider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

        [TestMethod]
        public void GetBinder_NullValueProviderResult_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(int));
            bindingContext.ValueProvider = new SimpleValueProvider(); // clear the ValueProvider

            TypeConverterModelBinderProvider provider = new TypeConverterModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = provider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

        [TestMethod]
        public void GetBinder_TypeConverterExistsFromString_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(int)); // TypeConverter exists Int32 -> String

            TypeConverterModelBinderProvider provider = new TypeConverterModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = provider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsInstanceOfType(binder, typeof(TypeConverterModelBinder));
        }

        private static ExtensibleModelBindingContext GetBindingContext(Type modelType) {
            return new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, modelType),
                ModelName = "theModelName",
                ValueProvider = new SimpleValueProvider() {
                    { "theModelName", "someValue" }
                }
            };
        }

    }
}
