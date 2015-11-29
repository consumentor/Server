namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class KeyValuePairModelBinderProviderTest {

        [TestMethod]
        public void GetBinder_CorrectModelTypeAndValueProviderEntries_ReturnsBinder() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(KeyValuePair<int, string>)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo.key", 42 },
                    { "foo.value", "someValue" }
                }
            };

            KeyValuePairModelBinderProvider binderProvider = new KeyValuePairModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsInstanceOfType(binder, typeof(KeyValuePairModelBinder<int, string>));
        }

        [TestMethod]
        public void GetBinder_ModelTypeIsIncorrect_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(List<int>)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo.key", 42 },
                    { "foo.value", "someValue" }
                }
            };

            KeyValuePairModelBinderProvider binderProvider = new KeyValuePairModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

        [TestMethod]
        public void GetBinder_ValueProviderDoesNotContainKeyProperty_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(KeyValuePair<int, string>)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo.value", "someValue" }
                }
            };

            KeyValuePairModelBinderProvider binderProvider = new KeyValuePairModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

        [TestMethod]
        public void GetBinder_ValueProviderDoesNotContainValueProperty_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(KeyValuePair<int, string>)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo.key", 42 }
                }
            };

            KeyValuePairModelBinderProvider binderProvider = new KeyValuePairModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

    }
}
