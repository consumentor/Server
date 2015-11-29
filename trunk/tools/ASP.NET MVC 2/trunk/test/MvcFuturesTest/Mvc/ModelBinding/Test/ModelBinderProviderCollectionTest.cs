namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class ModelBinderProviderCollectionTest {

        [TestMethod]
        public void ListWrappingConstructor() {
            // Arrange
            ModelBinderProvider[] providers = new ModelBinderProvider[] {
                new Mock<ModelBinderProvider>().Object,
                new Mock<ModelBinderProvider>().Object
            };

            // Act
            ModelBinderProviderCollection collection = new ModelBinderProviderCollection(providers);

            // Assert
            CollectionAssert.AreEqual(providers, collection);
        }

        [TestMethod]
        public void DefaultConstructor() {
            // Act
            ModelBinderProviderCollection collection = new ModelBinderProviderCollection();

            // Assert
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddNullProviderThrows() {
            // Arrange
            ModelBinderProviderCollection collection = new ModelBinderProviderCollection();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    collection.Add(null);
                },
                "item");
        }

        [TestMethod]
        public void RegisterBinderForGenericType_Factory() {
            // Arrange
            ModelBinderProvider mockProvider = new Mock<ModelBinderProvider>().Object;
            IExtensibleModelBinder mockBinder = new Mock<IExtensibleModelBinder>().Object;

            ModelBinderProviderCollection collection = new ModelBinderProviderCollection() {
                mockProvider
            };

            // Act
            collection.RegisterBinderForGenericType(typeof(List<>), _ => mockBinder);

            // Assert
            GenericModelBinderProvider genericProvider = collection[0] as GenericModelBinderProvider;
            Assert.IsNotNull(genericProvider, "Generic provider should've been inserted at the front of the list.");
            Assert.AreEqual(typeof(List<>), genericProvider.ModelType);
            Assert.AreEqual(mockProvider, collection[1]);
        }

        [TestMethod]
        public void RegisterBinderForGenericType_Instance() {
            // Arrange
            ModelBinderProvider mockProvider = new Mock<ModelBinderProvider>().Object;
            IExtensibleModelBinder mockBinder = new Mock<IExtensibleModelBinder>().Object;

            ModelBinderProviderCollection collection = new ModelBinderProviderCollection() {
                mockProvider
            };

            // Act
            collection.RegisterBinderForGenericType(typeof(List<>), mockBinder);

            // Assert
            GenericModelBinderProvider genericProvider = collection[0] as GenericModelBinderProvider;
            Assert.IsNotNull(genericProvider, "Generic provider should've been inserted at the front of the list.");
            Assert.AreEqual(typeof(List<>), genericProvider.ModelType);
            Assert.AreEqual(mockProvider, collection[1]);
        }

        [TestMethod]
        public void RegisterBinderForGenericType_Type() {
            // Arrange
            ModelBinderProvider mockProvider = new Mock<ModelBinderProvider>().Object;
            IExtensibleModelBinder mockBinder = new Mock<IExtensibleModelBinder>().Object;

            ModelBinderProviderCollection collection = new ModelBinderProviderCollection() {
                mockProvider
            };

            // Act
            collection.RegisterBinderForGenericType(typeof(List<>), typeof(CollectionModelBinder<>));

            // Assert
            GenericModelBinderProvider genericProvider = collection[0] as GenericModelBinderProvider;
            Assert.IsNotNull(genericProvider, "Generic provider should've been inserted at the front of the list.");
            Assert.AreEqual(typeof(List<>), genericProvider.ModelType);
            Assert.AreEqual(mockProvider, collection[1]);
        }

        [TestMethod]
        public void RegisterBinderForType_Factory() {
            // Arrange
            ModelBinderProvider mockProvider = new Mock<ModelBinderProvider>().Object;
            IExtensibleModelBinder mockBinder = new Mock<IExtensibleModelBinder>().Object;

            ModelBinderProviderCollection collection = new ModelBinderProviderCollection() {
                mockProvider
            };

            // Act
            collection.RegisterBinderForType(typeof(int), () => mockBinder);

            // Assert
            SimpleModelBinderProvider simpleProvider = collection[0] as SimpleModelBinderProvider;
            Assert.IsNotNull(simpleProvider, "Simple provider should've been inserted at the front of the list.");
            Assert.AreEqual(typeof(int), simpleProvider.ModelType);
            Assert.AreEqual(mockProvider, collection[1]);
        }

        [TestMethod]
        public void RegisterBinderForType_Instance() {
            // Arrange
            ModelBinderProvider mockProvider = new Mock<ModelBinderProvider>().Object;
            IExtensibleModelBinder mockBinder = new Mock<IExtensibleModelBinder>().Object;

            ModelBinderProviderCollection collection = new ModelBinderProviderCollection() {
                mockProvider
            };

            // Act
            collection.RegisterBinderForType(typeof(int), mockBinder);

            // Assert
            SimpleModelBinderProvider simpleProvider = collection[0] as SimpleModelBinderProvider;
            Assert.IsNotNull(simpleProvider, "Simple provider should've been inserted at the front of the list.");
            Assert.AreEqual(typeof(int), simpleProvider.ModelType);
            Assert.AreEqual(mockProvider, collection[1]);
        }

        [TestMethod]
        public void RegisterBinderForType_Instance_InsertsNewProviderBehindFrontOfListProviders() {
            // Arrange
            ModelBinderProvider frontOfListProvider = new ProviderAtFront();
            IExtensibleModelBinder mockBinder = new Mock<IExtensibleModelBinder>().Object;

            ModelBinderProviderCollection collection = new ModelBinderProviderCollection() {
                frontOfListProvider
            };

            // Act
            collection.RegisterBinderForType(typeof(int), mockBinder);

            // Assert
            CollectionAssert.AreEqual(
                new Type[] { typeof(ProviderAtFront), typeof(SimpleModelBinderProvider) },
                collection.Select(o => o.GetType()).ToArray(),
                "New provider should be inserted after any marked 'front of list'.");
        }

        [TestMethod]
        public void SetItem() {
            // Arrange
            ModelBinderProvider provider0 = new Mock<ModelBinderProvider>().Object;
            ModelBinderProvider provider1 = new Mock<ModelBinderProvider>().Object;
            ModelBinderProvider provider2 = new Mock<ModelBinderProvider>().Object;

            ModelBinderProviderCollection collection = new ModelBinderProviderCollection();
            collection.Add(provider0);
            collection.Add(provider1);

            // Act
            collection[1] = provider2;

            // Assert
            CollectionAssert.AreEqual(new ModelBinderProvider[] { provider0, provider2 }, collection);
        }

        [TestMethod]
        public void SetNullProviderThrows() {
            // Arrange
            ModelBinderProviderCollection collection = new ModelBinderProviderCollection();
            collection.Add(new Mock<ModelBinderProvider>().Object);

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    collection[0] = null;
                },
                "item");
        }

        [TestMethod]
        public void GetBinder_FromAttribute_BadAttribute_Throws() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(ModelWithProviderAttribute_BadAttribute))
            };

            ModelBinderProviderCollection providers = new ModelBinderProviderCollection();

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    providers.GetBinder(controllerContext, bindingContext);
                },
                @"The type 'System.Object' does not subclass Microsoft.Web.Mvc.ModelBinding.ModelBinderProvider or implement the interface Microsoft.Web.Mvc.ModelBinding.IExtensibleModelBinder.");
        }

        [TestMethod]
        public void GetBinder_FromAttribute_Binder_Generic_ReturnsBinder() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(ModelWithProviderAttribute_Binder_Generic<int>)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo", "fooValue" }
                }
            };

            ModelBinderProviderCollection providers = new ModelBinderProviderCollection();
            providers.RegisterBinderForType(typeof(ModelWithProviderAttribute_Binder_Generic<int>), new Mock<IExtensibleModelBinder>().Object, true /* suppressPrefix */);

            // Act
            IExtensibleModelBinder binder = providers.GetBinder(controllerContext, bindingContext);

            // Assert
            Assert.AreEqual(typeof(CustomGenericBinder<int>), binder.GetType(), "Binder should've come from attribute rather than collection.");
        }

        [TestMethod]
        public void GetBinder_FromAttribute_Binder_SuppressPrefixCheck_ReturnsBinder() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(ModelWithProviderAttribute_Binder_SuppressPrefix)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "bar", "barValue" }
                }
            };

            ModelBinderProviderCollection providers = new ModelBinderProviderCollection();
            providers.RegisterBinderForType(typeof(ModelWithProviderAttribute_Binder_SuppressPrefix), new Mock<IExtensibleModelBinder>().Object, true /* suppressPrefix */);

            // Act
            IExtensibleModelBinder binder = providers.GetBinder(controllerContext, bindingContext);

            // Assert
            Assert.AreEqual(typeof(CustomBinder), binder.GetType(), "Binder should've come from attribute rather than collection.");
        }

        [TestMethod]
        public void GetBinder_FromAttribute_Binder_ValueNotPresent_ReturnsNull() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(ModelWithProviderAttribute_Binder)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "bar", "barValue" }
                }
            };

            ModelBinderProviderCollection providers = new ModelBinderProviderCollection();
            providers.RegisterBinderForType(typeof(ModelWithProviderAttribute_Binder), new Mock<IExtensibleModelBinder>().Object, true /* suppressPrefix */);

            // Act
            IExtensibleModelBinder binder = providers.GetBinder(controllerContext, bindingContext);

            // Assert
            Assert.IsNull(binder, "Binder should've come from attribute rather than collection, even if the attribute's provider returns null.");
        }

        [TestMethod]
        public void GetBinder_FromAttribute_Binder_ValuePresent_ReturnsBinder() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(ModelWithProviderAttribute_Binder)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo", "fooValue" }
                }
            };

            ModelBinderProviderCollection providers = new ModelBinderProviderCollection();
            providers.RegisterBinderForType(typeof(ModelWithProviderAttribute_Binder), new Mock<IExtensibleModelBinder>().Object, true /* suppressPrefix */);

            // Act
            IExtensibleModelBinder binder = providers.GetBinder(controllerContext, bindingContext);

            // Assert
            Assert.AreEqual(typeof(CustomBinder), binder.GetType(), "Binder should've come from attribute rather than collection.");
        }

        [TestMethod]
        public void GetBinder_FromAttribute_Provider_ReturnsBinder() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(ModelWithProviderAttribute_Provider))
            };

            ModelBinderProviderCollection providers = new ModelBinderProviderCollection();
            providers.RegisterBinderForType(typeof(ModelWithProviderAttribute_Provider), new Mock<IExtensibleModelBinder>().Object, true /* suppressPrefix */);

            // Act
            IExtensibleModelBinder binder = providers.GetBinder(controllerContext, bindingContext);

            // Assert
            Assert.AreEqual(typeof(CustomBinder), binder.GetType(), "Binder should've come from attribute rather than collection.");
        }

        [TestMethod]
        public void GetBinderReturnsFirstBinderFromProviders() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(object))
            };
            IExtensibleModelBinder expectedBinder = new Mock<IExtensibleModelBinder>().Object;

            Mock<ModelBinderProvider> mockProvider = new Mock<ModelBinderProvider>();
            mockProvider.Expect(p => p.GetBinder(controllerContext, bindingContext)).Returns(expectedBinder);

            ModelBinderProviderCollection collection = new ModelBinderProviderCollection(new ModelBinderProvider[] {
                new Mock<ModelBinderProvider>().Object,
                mockProvider.Object,
                new Mock<ModelBinderProvider>().Object
            });

            // Act
            IExtensibleModelBinder returned = collection.GetBinder(controllerContext, bindingContext);

            // Assert
            Assert.AreEqual(expectedBinder, returned);
        }

        [TestMethod]
        public void GetBinderReturnsNullIfNoProviderMatches() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(object))
            };

            ModelBinderProviderCollection collection = new ModelBinderProviderCollection(new ModelBinderProvider[] {
                new Mock<ModelBinderProvider>().Object,
            });

            // Act
            IExtensibleModelBinder returned = collection.GetBinder(controllerContext, bindingContext);

            // Assert
            Assert.IsNull(returned);
        }

        [TestMethod]
        public void GetBinderThrowsIfBindingContextIsNull() {
            // Arrange
            ModelBinderProviderCollection collection = new ModelBinderProviderCollection();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    collection.GetBinder(new ControllerContext(), null);
                }, "bindingContext");
        }

        [TestMethod]
        public void GetBinderThrowsIfControllerContextIsNull() {
            // Arrange
            ModelBinderProviderCollection collection = new ModelBinderProviderCollection();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    collection.GetBinder(null, new ExtensibleModelBindingContext());
                }, "controllerContext");
        }

        [TestMethod]
        public void GetBinderThrowsIfModelTypeHasBindAttribute() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(ModelWithBindAttribute))
            };
            ModelBinderProviderCollection collection = new ModelBinderProviderCollection();

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    collection.GetBinder(controllerContext, bindingContext);
                },
                @"The model of type 'Microsoft.Web.Mvc.ModelBinding.Test.ModelBinderProviderCollectionTest+ModelWithBindAttribute' has a [Bind] attribute. The new model binding system cannot be used with models that have type-level [Bind] attributes. Use the [BindRequired] and [BindNever] attributes on the model type or its properties instead.");
        }

        [TestMethod]
        public void GetRequiredBinderThrowsIfNoProviderMatches() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(int))
            };

            ModelBinderProviderCollection collection = new ModelBinderProviderCollection(new ModelBinderProvider[] {
                new Mock<ModelBinderProvider>().Object,
            });

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    collection.GetRequiredBinder(controllerContext, bindingContext);
                },
                @"A binder for type System.Int32 could not be located.");
        }

        [MetadataType(typeof(ModelWithBindAttribute_Buddy))]
        private class ModelWithBindAttribute {

            [Bind]
            private class ModelWithBindAttribute_Buddy {
            }
        }

        [ModelBinderProviderOptions(FrontOfList = true)]
        private class ProviderAtFront : ModelBinderProvider {
            public override IExtensibleModelBinder GetBinder(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
                throw new NotImplementedException();
            }
        }

        [ExtensibleModelBinder(typeof(object))]
        private class ModelWithProviderAttribute_BadAttribute { }

        [ExtensibleModelBinder(typeof(CustomBinder))]
        private class ModelWithProviderAttribute_Binder { }

        [ExtensibleModelBinder(typeof(CustomGenericBinder<>))]
        private class ModelWithProviderAttribute_Binder_Generic<T> { }

        [ExtensibleModelBinder(typeof(CustomBinder), SuppressPrefixCheck = true)]
        private class ModelWithProviderAttribute_Binder_SuppressPrefix { }

        [ExtensibleModelBinder(typeof(CustomProvider))]
        private class ModelWithProviderAttribute_Provider { }

        private class CustomProvider : ModelBinderProvider {
            public override IExtensibleModelBinder GetBinder(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
                return new CustomBinder();
            }
        }

        private class CustomBinder : IExtensibleModelBinder {
            public bool BindModel(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
                throw new NotImplementedException();
            }
        }

        private class CustomGenericBinder<T> : IExtensibleModelBinder {
            public bool BindModel(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
                throw new NotImplementedException();
            }
        }

    }
}
