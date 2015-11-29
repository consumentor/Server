namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class TypeMatchModelBinderProviderTest {

        [TestMethod]
        public void ProviderIsMarkedFrontOfList() {
            // Arrange
            Type t = typeof(TypeMatchModelBinderProvider);

            // Act & assert
            Assert.IsTrue(t.GetCustomAttributes(typeof(ModelBinderProviderOptionsAttribute), true /* inherit */).Cast<ModelBinderProviderOptionsAttribute>().Single().FrontOfList,
                "Provider should have been marked 'front of list'.");
        }

        [TestMethod]
        public void GetBinder_InvalidValueProviderResult_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = GetBindingContext();
            bindingContext.ValueProvider = new SimpleValueProvider() {
                { "theModelName", "not an integer" }
            };

            TypeMatchModelBinderProvider provider = new TypeMatchModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = provider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

        [TestMethod]
        public void BindModel_ValidValueProviderResult_ReturnsBinder() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = GetBindingContext();
            bindingContext.ValueProvider = new SimpleValueProvider() {
                { "theModelName", 42 }
            };

            TypeMatchModelBinderProvider provider = new TypeMatchModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = provider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsInstanceOfType(binder, typeof(TypeMatchModelBinder));
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
