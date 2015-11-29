namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class CollectionModelBinderProviderTest {

        [TestMethod]
        public void GetBinder_CorrectModelTypeAndValueProviderEntries_ReturnsBinder() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(IEnumerable<int>)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo[0]", "42" },
                }
            };

            CollectionModelBinderProvider binderProvider = new CollectionModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsInstanceOfType(binder, typeof(CollectionModelBinder<int>));
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

            CollectionModelBinderProvider binderProvider = new CollectionModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

        [TestMethod]
        public void GetBinder_ValueProviderDoesNotContainPrefix_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(IEnumerable<int>)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider()
            };

            CollectionModelBinderProvider binderProvider = new CollectionModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

    }
}
