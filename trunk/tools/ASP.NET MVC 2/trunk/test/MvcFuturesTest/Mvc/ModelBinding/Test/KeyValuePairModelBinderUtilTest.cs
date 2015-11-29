namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class KeyValuePairModelBinderUtilTest {

        [TestMethod]
        public void TryBindStrongModel_BinderExists_BinderReturnsCorrectlyTypedObject_ReturnsTrue() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(int)),
                ModelName = "someName",
                ModelState = new ModelStateDictionary(),
                ModelBinderProviders = new ModelBinderProviderCollection(),
                ValueProvider = new SimpleValueProvider()
            };

            Mock<IExtensibleModelBinder> mockIntBinder = new Mock<IExtensibleModelBinder>();
            mockIntBinder
                .Expect(o => o.BindModel(controllerContext, It.IsAny<ExtensibleModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ExtensibleModelBindingContext mbc) {
                        Assert.AreEqual("someName.key", mbc.ModelName);
                        mbc.Model = 42;
                        return true;
                    });
            bindingContext.ModelBinderProviders.RegisterBinderForType(typeof(int), mockIntBinder.Object, true /* suppressPrefixCheck */);

            // Act
            int model;
            bool retVal = KeyValuePairModelBinderUtil.TryBindStrongModel<int>(controllerContext, bindingContext, "key", new EmptyModelMetadataProvider(), out model);

            // Assert
            Assert.IsTrue(retVal);
            Assert.AreEqual(42, model);
            Assert.AreEqual(1, bindingContext.ValidationNode.ChildNodes.Count, "Child validation node should have been added.");
            Assert.AreEqual(0, bindingContext.ModelState.Count, "ModelState should remain unmodified.");
        }

        [TestMethod]
        public void TryBindStrongModel_BinderExists_BinderReturnsIncorrectlyTypedObject_ReturnsTrue() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(int)),
                ModelName = "someName",
                ModelState = new ModelStateDictionary(),
                ModelBinderProviders = new ModelBinderProviderCollection(),
                ValueProvider = new SimpleValueProvider()
            };

            Mock<IExtensibleModelBinder> mockIntBinder = new Mock<IExtensibleModelBinder>();
            mockIntBinder
                .Expect(o => o.BindModel(controllerContext, It.IsAny<ExtensibleModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ExtensibleModelBindingContext mbc) {
                        Assert.AreEqual("someName.key", mbc.ModelName);
                        return true;
                    });
            bindingContext.ModelBinderProviders.RegisterBinderForType(typeof(int), mockIntBinder.Object, true /* suppressPrefixCheck */);

            // Act
            int model;
            bool retVal = KeyValuePairModelBinderUtil.TryBindStrongModel<int>(controllerContext, bindingContext, "key", new EmptyModelMetadataProvider(), out model);

            // Assert
            Assert.IsTrue(retVal);
            Assert.AreEqual(default(int), model);
            Assert.AreEqual(1, bindingContext.ValidationNode.ChildNodes.Count, "Child validation node should have been added.");
            Assert.AreEqual(0, bindingContext.ModelState.Count, "ModelState should remain unmodified.");
        }

        [TestMethod]
        public void TryBindStrongModel_NoBinder_ReturnsFalse() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(int)),
                ModelName = "someName",
                ModelState = new ModelStateDictionary(),
                ModelBinderProviders = new ModelBinderProviderCollection(),
                ValueProvider = new SimpleValueProvider()
            };

            // Act
            int model;
            bool retVal = KeyValuePairModelBinderUtil.TryBindStrongModel<int>(controllerContext, bindingContext, "key", new EmptyModelMetadataProvider(), out model);

            // Assert
            Assert.IsFalse(retVal);
            Assert.AreEqual(default(int), model);
            Assert.AreEqual(0, bindingContext.ValidationNode.ChildNodes.Count, "Child validation node should not have been added.");
            Assert.AreEqual(0, bindingContext.ModelState.Count, "ModelState should remain unmodified.");
        }

    }
}
