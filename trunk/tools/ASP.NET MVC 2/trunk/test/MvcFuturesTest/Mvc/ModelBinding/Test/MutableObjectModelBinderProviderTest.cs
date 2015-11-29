namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class MutableObjectModelBinderProviderTest {

        [TestMethod]
        public void GetBinder_NoPrefixInValueProvider_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => 42, typeof(int)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider()
            };

            MutableObjectModelBinderProvider binderProvider = new MutableObjectModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

        [TestMethod]
        public void GetBinder_PrefixInValueProvider_ReturnsBinder() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => 42, typeof(int)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo.bar", "someValue" }
                }
            };

            MutableObjectModelBinderProvider binderProvider = new MutableObjectModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNotNull(binder);
            Assert.AreEqual(typeof(MutableObjectModelBinder), binder.GetType());
        }

        [TestMethod]
        public void GetBinder_TypeIsComplexModelDto_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(ComplexModelDto)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo.bar", "someValue" }
                }
            };

            MutableObjectModelBinderProvider binderProvider = new MutableObjectModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

    }
}
