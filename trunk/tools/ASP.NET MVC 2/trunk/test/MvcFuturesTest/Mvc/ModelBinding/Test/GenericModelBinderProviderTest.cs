namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class GenericModelBinderProviderTest {

        [TestMethod]
        public void Constructor_WithFactory_ThrowsIfModelBinderFactoryIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new GenericModelBinderProvider(typeof(List<>), (Func<Type[], IExtensibleModelBinder>)null);
                }, "modelBinderFactory");
        }

        [TestMethod]
        public void Constructor_WithFactory_ThrowsIfModelTypeIsNotOpenGeneric() {
            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new GenericModelBinderProvider(typeof(List<int>), _ => null);
                },
                @"The type 'System.Collections.Generic.List`1[System.Int32]' is not an open generic type.
Parameter name: modelType");
        }

        [TestMethod]
        public void Constructor_WithFactory_ThrowsIfModelTypeIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new GenericModelBinderProvider(null, _ => null);
                }, "modelType");
        }

        [TestMethod]
        public void Constructor_WithInstance_ThrowsIfModelBinderIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new GenericModelBinderProvider(typeof(List<>), (IExtensibleModelBinder)null);
                }, "modelBinder");
        }

        [TestMethod]
        public void Constructor_WithInstance_ThrowsIfModelTypeIsNotOpenGeneric() {
            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new GenericModelBinderProvider(typeof(List<int>), new MutableObjectModelBinder());
                },
                @"The type 'System.Collections.Generic.List`1[System.Int32]' is not an open generic type.
Parameter name: modelType");
        }

        [TestMethod]
        public void Constructor_WithInstance_ThrowsIfModelTypeIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new GenericModelBinderProvider(null, new MutableObjectModelBinder());
                }, "modelType");
        }

        [TestMethod]
        public void Constructor_WithType_ThrowsIfModelBinderTypeIsNotModelBinder() {
            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new GenericModelBinderProvider(typeof(List<>), typeof(string));
                },
                @"The type 'System.String' does not implement the interface 'Microsoft.Web.Mvc.ModelBinding.IExtensibleModelBinder'.
Parameter name: modelBinderType");
        }

        [TestMethod]
        public void Constructor_WithType_ThrowsIfModelBinderTypeIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new GenericModelBinderProvider(typeof(List<>), (Type)null);
                }, "modelBinderType");
        }

        [TestMethod]
        public void Constructor_WithType_ThrowsIfModelBinderTypeTypeArgumentMismatch() {
            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new GenericModelBinderProvider(typeof(List<>), typeof(DictionaryModelBinder<,>));
                },
                @"The open model type 'System.Collections.Generic.List`1[T]' has 1 generic type argument(s), but the open binder type 'Microsoft.Web.Mvc.ModelBinding.DictionaryModelBinder`2[TKey,TValue]' has 2 generic type argument(s). The binder type must not be an open generic type or must have the same number of generic arguments as the open model type.
Parameter name: modelBinderType");
        }

        [TestMethod]
        public void Constructor_WithType_ThrowsIfModelTypeIsNotOpenGeneric() {
            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new GenericModelBinderProvider(typeof(List<int>), typeof(MutableObjectModelBinder));
                },
                @"The type 'System.Collections.Generic.List`1[System.Int32]' is not an open generic type.
Parameter name: modelType");
        }

        [TestMethod]
        public void Constructor_WithType_ThrowsIfModelTypeIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new GenericModelBinderProvider(null, typeof(MutableObjectModelBinder));
                }, "modelType");
        }

        [TestMethod]
        public void GetBinder_TypeDoesNotMatch_ModelTypeIsInterface_ReturnsNull() {
            // Arrange
            GenericModelBinderProvider provider = new GenericModelBinderProvider(typeof(IEnumerable<>), typeof(CollectionModelBinder<>)) {
                SuppressPrefixCheck = true
            };
            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(object));

            // Act
            IExtensibleModelBinder binder = provider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(binder);
        }

        [TestMethod]
        public void GetBinder_TypeDoesNotMatch_ModelTypeIsNotInterface_ReturnsNull() {
            // Arrange
            GenericModelBinderProvider provider = new GenericModelBinderProvider(typeof(List<>), typeof(CollectionModelBinder<>)) {
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
            GenericModelBinderProvider provider = new GenericModelBinderProvider(typeof(List<>), binderInstance);

            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(List<int>));
            bindingContext.ValueProvider = new SimpleValueProvider();

            // Act
            IExtensibleModelBinder returnedBinder = provider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsNull(returnedBinder);
        }

        [TestMethod]
        public void GetBinder_TypeMatches_Success_Factory_ReturnsBinder() {
            // Arrange
            IExtensibleModelBinder binderInstance = new Mock<IExtensibleModelBinder>().Object;

            Func<Type[], IExtensibleModelBinder> binderFactory = typeArguments => {
                CollectionAssert.AreEqual(new Type[] { typeof(int) }, typeArguments);
                return binderInstance;
            };

            GenericModelBinderProvider provider = new GenericModelBinderProvider(typeof(IList<>), binderFactory) {
                SuppressPrefixCheck = true
            };

            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(List<int>));

            // Act
            IExtensibleModelBinder returnedBinder = provider.GetBinder(null, bindingContext);

            // Assert
            Assert.AreSame(binderInstance, returnedBinder);
        }

        [TestMethod]
        public void GetBinder_TypeMatches_Success_Instance_ReturnsBinder() {
            // Arrange
            IExtensibleModelBinder binderInstance = new Mock<IExtensibleModelBinder>().Object;

            GenericModelBinderProvider provider = new GenericModelBinderProvider(typeof(List<>), binderInstance) {
                SuppressPrefixCheck = true
            };

            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(List<int>));

            // Act
            IExtensibleModelBinder returnedBinder = provider.GetBinder(null, bindingContext);

            // Assert
            Assert.AreSame(binderInstance, returnedBinder);
        }

        [TestMethod]
        public void GetBinder_TypeMatches_Success_TypeActivation_ReturnsBinder() {
            // Arrange
            GenericModelBinderProvider provider = new GenericModelBinderProvider(typeof(List<>), typeof(CollectionModelBinder<>)) {
                SuppressPrefixCheck = true
            };

            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(List<int>));

            // Act
            IExtensibleModelBinder returnedBinder = provider.GetBinder(null, bindingContext);

            // Assert
            Assert.IsInstanceOfType(returnedBinder, typeof(CollectionModelBinder<int>));
        }

        [TestMethod]
        public void GetBinderThrowsIfBindingContextIsNull() {
            // Arrange
            GenericModelBinderProvider provider = new GenericModelBinderProvider(typeof(IEnumerable<>), typeof(CollectionModelBinder<>));

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    provider.GetBinder(null, null);
                }, "bindingContext");
        }

        private static ExtensibleModelBindingContext GetBindingContext(Type modelType) {
            return new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => null, modelType)
            };
        }

    }
}
