namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class SimpleModelBinderProviderTest {

        [TestMethod]
        public void ConstructorWithFactoryThrowsIfModelBinderFactoryIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new SimpleModelBinderProvider(typeof(object), (Func<IExtensibleModelBinder>)null);
                }, "modelBinderFactory");
        }

        [TestMethod]
        public void ConstructorWithFactoryThrowsIfModelTypeIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new SimpleModelBinderProvider(null, () => null);
                }, "modelType");
        }

        [TestMethod]
        public void ConstructorWithInstanceThrowsIfModelBinderIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new SimpleModelBinderProvider(typeof(object), (IExtensibleModelBinder)null);
                }, "modelBinder");
        }

        [TestMethod]
        public void ConstructorWithInstanceThrowsIfModelTypeIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new SimpleModelBinderProvider(null, new Mock<IExtensibleModelBinder>().Object);
                }, "modelType");
        }

        [TestMethod]
        public void GetBinder_TypeDoesNotMatch_ReturnsNull() {
            // Arrange
            SimpleModelBinderProvider provider = new SimpleModelBinderProvider(typeof(string), new Mock<IExtensibleModelBinder>().Object) {
                SuppressPrefixCheck = true
            };
            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(object));

            // Act
            IExtensibleModelBinder binder = provider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

        [TestMethod]
        public void GetBinder_TypeMatches_PrefixNotFound_ReturnsNull() {
            // Arrange
            IExtensibleModelBinder binderInstance = new Mock<IExtensibleModelBinder>().Object;
            SimpleModelBinderProvider provider = new SimpleModelBinderProvider(typeof(string), binderInstance);

            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(string));
            bindingContext.ValueProvider = new SimpleValueProvider();

            // Act
            IExtensibleModelBinder returnedBinder = provider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(returnedBinder);
        }

        [TestMethod]
        public void GetBinder_TypeMatches_PrefixSuppressed_ReturnsFactoryInstance() {
            // Arrange
            int numExecutions = 0;
            IExtensibleModelBinder theBinderInstance = new Mock<IExtensibleModelBinder>().Object;
            Func<IExtensibleModelBinder> factory = delegate {
                numExecutions++;
                return theBinderInstance;
            };

            SimpleModelBinderProvider provider = new SimpleModelBinderProvider(typeof(string), factory) {
                SuppressPrefixCheck = true
            };
            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(string));

            // Act
            IExtensibleModelBinder returnedBinder = provider.GetBinder(null, bindingContext);
            returnedBinder = provider.GetBinder(null, bindingContext);

            // Assert
            Assert.AreEqual(2, numExecutions, "Factory didn't execute correct number of times.");
            Assert.AreEqual(theBinderInstance, returnedBinder);
        }

        [TestMethod]
        public void GetBinder_TypeMatches_PrefixSuppressed_ReturnsInstance() {
            // Arrange
            IExtensibleModelBinder theBinderInstance = new Mock<IExtensibleModelBinder>().Object;
            SimpleModelBinderProvider provider = new SimpleModelBinderProvider(typeof(string), theBinderInstance) {
                SuppressPrefixCheck = true
            };
            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(string));

            // Act
            IExtensibleModelBinder returnedBinder = provider.GetBinder(null, bindingContext);

            // Assert
            Assert.AreEqual(theBinderInstance, returnedBinder);
        }

        [TestMethod]
        public void GetBinderThrowsIfBindingContextIsNull() {
            // Arrange
            SimpleModelBinderProvider provider = new SimpleModelBinderProvider(typeof(string), new Mock<IExtensibleModelBinder>().Object);

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    provider.GetBinder(null, null);
                }, "bindingContext");
        }

        [TestMethod]
        public void ModelTypeProperty() {
            // Arrange
            SimpleModelBinderProvider provider = new SimpleModelBinderProvider(typeof(string), new Mock<IExtensibleModelBinder>().Object);

            // Act & assert
            Assert.AreEqual(typeof(string), provider.ModelType);
        }

        private static ExtensibleModelBindingContext GetBindingContext(Type modelType) {
            return new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => null, modelType)
            };
        }

    }
}
