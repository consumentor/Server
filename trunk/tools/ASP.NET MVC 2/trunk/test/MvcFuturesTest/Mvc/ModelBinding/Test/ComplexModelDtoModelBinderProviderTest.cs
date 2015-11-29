namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ComplexModelDtoModelBinderProviderTest {

        [TestMethod]
        public void GetBinder_TypeDoesNotMatch_ReturnsNull() {
            // Arrange
            ComplexModelDtoModelBinderProvider provider = new ComplexModelDtoModelBinderProvider();
            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(object));

            // Act
            IExtensibleModelBinder binder = provider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

        [TestMethod]
        public void GetBinder_TypeMatches_ReturnsBinder() {
            // Arrange
            ComplexModelDtoModelBinderProvider provider = new ComplexModelDtoModelBinderProvider();
            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(ComplexModelDto));

            // Act
            IExtensibleModelBinder binder = provider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsInstanceOfType(binder, typeof(ComplexModelDtoModelBinder));
        }

        private static ExtensibleModelBindingContext GetBindingContext(Type modelType) {
            return new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => null, modelType)
            };
        }

    }
}
