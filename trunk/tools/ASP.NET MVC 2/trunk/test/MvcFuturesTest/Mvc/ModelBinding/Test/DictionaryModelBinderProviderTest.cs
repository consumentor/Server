namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class DictionaryModelBinderProviderTest {

        [TestMethod]
        public void GetBinder_CorrectModelTypeAndValueProviderEntries_ReturnsBinder() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(IDictionary<int, string>)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo[0]", "42" },
                }
            };

            DictionaryModelBinderProvider binderProvider = new DictionaryModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsInstanceOfType(binder, typeof(DictionaryModelBinder<int, string>));
        }

        [TestMethod]
        public void GetBinder_ModelTypeIsIncorrect_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(int)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo[0]", "42" },
                }
            };

            DictionaryModelBinderProvider binderProvider = new DictionaryModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

        [TestMethod]
        public void GetBinder_ValueProviderDoesNotContainPrefix_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(IDictionary<int, string>)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider()
            };

            DictionaryModelBinderProvider binderProvider = new DictionaryModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

    }
}
