namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class ArrayModelBinderProviderTest {

        [TestMethod]
        public void GetBinder_CorrectModelTypeAndValueProviderEntries_ReturnsBinder() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(int[])),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo[0]", "42" },
                }
            };

            ArrayModelBinderProvider binderProvider = new ArrayModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsInstanceOfType(binder, typeof(ArrayModelBinder<int>));
        }

        [TestMethod]
        public void GetBinder_ModelMetadataReturnsReadOnly_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(int[])),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo[0]", "42" },
                }
            };
            bindingContext.ModelMetadata.IsReadOnly = true;

            ArrayModelBinderProvider binderProvider = new ArrayModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

        [TestMethod]
        public void GetBinder_ModelTypeIsIncorrect_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(ICollection<int>)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo[0]", "42" },
                }
            };

            ArrayModelBinderProvider binderProvider = new ArrayModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

        [TestMethod]
        public void GetBinder_ValueProviderDoesNotContainPrefix_ReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(int[])),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider()
            };

            ArrayModelBinderProvider binderProvider = new ArrayModelBinderProvider();

            // Act
            IExtensibleModelBinder binder = binderProvider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

    }
}
