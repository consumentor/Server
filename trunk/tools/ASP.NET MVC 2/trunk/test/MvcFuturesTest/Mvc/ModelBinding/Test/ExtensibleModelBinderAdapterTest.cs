namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class ExtensibleModelBinderAdapterTest {

        [TestMethod]
        public void BindModel_PropertyFilterIsSet_Throws() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();

            ModelBindingContext bindingContext = new ModelBindingContext() {
                FallbackToEmptyPrefix = true,
                ModelMetadata = new DataAnnotationsModelMetadataProvider().GetMetadataForType(null, typeof(SimpleModel)),
                PropertyFilter = (new BindAttribute() { Include = "FirstName " }).IsPropertyAllowed
            };

            ModelBinderProviderCollection binderProviders = new ModelBinderProviderCollection();
            ExtensibleModelBinderAdapter shimBinder = new ExtensibleModelBinderAdapter(binderProviders);

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    shimBinder.BindModel(controllerContext, bindingContext);
                },
                @"The new model binding system cannot be used when a property whitelist or blacklist has been specified in [Bind] or via the call to UpdateModel() / TryUpdateModel(). Use the [BindRequired] and [BindNever] attributes on the model type or its properties instead.");
        }

        [TestMethod]
        public void BindModel_SuccessfulBind_RunsValidationAndReturnsModel() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();
            bool validationCalled = false;

            ModelBindingContext bindingContext = new ModelBindingContext() {
                FallbackToEmptyPrefix = true,
                ModelMetadata = new DataAnnotationsModelMetadataProvider().GetMetadataForType(null, typeof(int)),
                ModelName = "someName",
                ModelState = controllerContext.Controller.ViewData.ModelState,
                PropertyFilter = _ => true,
                ValueProvider = new SimpleValueProvider() {
                    { "someName", "dummyValue" }
                }
            };

            Mock<IExtensibleModelBinder> mockIntBinder = new Mock<IExtensibleModelBinder>();
            mockIntBinder
                .Expect(o => o.BindModel(controllerContext, It.IsAny<ExtensibleModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ExtensibleModelBindingContext mbc) {
                        Assert.AreSame(bindingContext.ModelMetadata, mbc.ModelMetadata, "ModelMetadata wasn't correct.");
                        Assert.AreEqual("someName", mbc.ModelName, "ModelName wasn't correct.");
                        Assert.AreSame(bindingContext.ValueProvider, mbc.ValueProvider, "ValueProvider wasn't correct.");

                        mbc.Model = 42;
                        mbc.ValidationNode.Validating += delegate { validationCalled = true; };
                        return true;
                    });

            ModelBinderProviderCollection binderProviders = new ModelBinderProviderCollection();
            binderProviders.RegisterBinderForType(typeof(int), mockIntBinder.Object, false /* suppressPrefixCheck */);
            ExtensibleModelBinderAdapter shimBinder = new ExtensibleModelBinderAdapter(binderProviders);

            // Act
            object retVal = shimBinder.BindModel(controllerContext, bindingContext);

            // Assert
            Assert.AreEqual(42, retVal);
            Assert.IsTrue(validationCalled);
            Assert.IsTrue(bindingContext.ModelState.IsValid, "No errors should have been added to ModelState.");
        }

        [TestMethod]
        public void BindModel_SuccessfulBind_ComplexTypeFallback_RunsValidationAndReturnsModel() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();

            bool validationCalled = false;
            List<int> expectedModel = new List<int>() { 1, 2, 3, 4, 5 };

            ModelBindingContext bindingContext = new ModelBindingContext() {
                FallbackToEmptyPrefix = true,
                ModelMetadata = new DataAnnotationsModelMetadataProvider().GetMetadataForType(null, typeof(List<int>)),
                ModelName = "someName",
                ModelState = controllerContext.Controller.ViewData.ModelState,
                PropertyFilter = _ => true,
                ValueProvider = new SimpleValueProvider() {
                    { "someOtherName", "dummyValue" }
                }
            };

            Mock<IExtensibleModelBinder> mockIntBinder = new Mock<IExtensibleModelBinder>();
            mockIntBinder
                .Expect(o => o.BindModel(controllerContext, It.IsAny<ExtensibleModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ExtensibleModelBindingContext mbc) {
                        Assert.AreSame(bindingContext.ModelMetadata, mbc.ModelMetadata, "ModelMetadata wasn't correct.");
                        Assert.AreEqual("", mbc.ModelName, "ModelName wasn't correct.");
                        Assert.AreSame(bindingContext.ValueProvider, mbc.ValueProvider, "ValueProvider wasn't correct.");

                        mbc.Model = expectedModel;
                        mbc.ValidationNode.Validating += delegate { validationCalled = true; };
                        return true;
                    });

            ModelBinderProviderCollection binderProviders = new ModelBinderProviderCollection();
            binderProviders.RegisterBinderForType(typeof(List<int>), mockIntBinder.Object, false /* suppressPrefixCheck */);
            ExtensibleModelBinderAdapter shimBinder = new ExtensibleModelBinderAdapter(binderProviders);

            // Act
            object retVal = shimBinder.BindModel(controllerContext, bindingContext);

            // Assert
            Assert.AreEqual(expectedModel, retVal);
            Assert.IsTrue(validationCalled);
            Assert.IsTrue(bindingContext.ModelState.IsValid, "No errors should have been added to ModelState.");
        }

        [TestMethod]
        public void BindModel_UnsuccessfulBind_BinderFails_ReturnsNull() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();
            Mock<IExtensibleModelBinder> mockListBinder = new Mock<IExtensibleModelBinder>();
            mockListBinder.Expect(o => o.BindModel(controllerContext, It.IsAny<ExtensibleModelBindingContext>())).Returns(false).Verifiable();

            ModelBinderProviderCollection binderProviders = new ModelBinderProviderCollection();
            binderProviders.RegisterBinderForType(typeof(List<int>), mockListBinder.Object, true /* suppressPrefixCheck */);
            ExtensibleModelBinderAdapter shimBinder = new ExtensibleModelBinderAdapter(binderProviders);

            ModelBindingContext bindingContext = new ModelBindingContext() {
                FallbackToEmptyPrefix = false,
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(List<int>)),
                ModelState = controllerContext.Controller.ViewData.ModelState
            };

            // Act
            object retVal = shimBinder.BindModel(controllerContext, bindingContext);

            // Assert
            Assert.IsNull(retVal);
            Assert.IsTrue(bindingContext.ModelState.IsValid, "No errors should have been added to ModelState.");
            mockListBinder.Verify();
        }

        [TestMethod]
        public void BindModel_UnsuccessfulBind_SimpleTypeNoFallback_ReturnsNull() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();
            Mock<ModelBinderProvider> mockBinderProvider = new Mock<ModelBinderProvider>();
            mockBinderProvider.Expect(o => o.GetBinder(controllerContext, It.IsAny<ExtensibleModelBindingContext>())).Returns((IExtensibleModelBinder)null).AtMostOnce().Verifiable();
            ModelBinderProviderCollection binderProviders = new ModelBinderProviderCollection() {
                mockBinderProvider.Object
            };
            ExtensibleModelBinderAdapter shimBinder = new ExtensibleModelBinderAdapter(binderProviders);

            ModelBindingContext bindingContext = new ModelBindingContext() {
                FallbackToEmptyPrefix = true,
                ModelMetadata = new DataAnnotationsModelMetadataProvider().GetMetadataForType(null, typeof(int)),
                ModelState = controllerContext.Controller.ViewData.ModelState
            };

            // Act
            object retVal = shimBinder.BindModel(controllerContext, bindingContext);

            // Assert
            Assert.IsNull(retVal);
            Assert.IsTrue(bindingContext.ModelState.IsValid, "No errors should have been added to ModelState.");
            mockBinderProvider.Verify();
        }

        private static ControllerContext GetControllerContext() {
            return new ControllerContext() {
                Controller = new SimpleController()
            };
        }

        private class SimpleController : Controller { }

        private class SimpleModel {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

    }
}
