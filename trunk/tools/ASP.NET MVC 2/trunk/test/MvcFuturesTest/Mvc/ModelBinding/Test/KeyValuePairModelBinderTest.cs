namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class KeyValuePairModelBinderTest {

        [TestMethod]
        public void BindModel_MissingKey_ReturnsFalse() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(KeyValuePair<int, string>)),
                ModelName = "someName",
                ModelBinderProviders = new ModelBinderProviderCollection(),
                ValueProvider = new SimpleValueProvider()
            };

            KeyValuePairModelBinder<int, string> binder = new KeyValuePairModelBinder<int, string>();

            // Act
            bool retVal = binder.BindModel(controllerContext, bindingContext);

            // Assert
            Assert.IsFalse(retVal);
            Assert.IsNull(bindingContext.Model);
            Assert.AreEqual(0, bindingContext.ValidationNode.ChildNodes.Count, "No child nodes should have been added.");
        }

        [TestMethod]
        public void BindModel_MissingValue_ReturnsTrue() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(KeyValuePair<int, string>)),
                ModelName = "someName",
                ModelBinderProviders = new ModelBinderProviderCollection(),
                ValueProvider = new SimpleValueProvider()
            };

            Mock<IExtensibleModelBinder> mockIntBinder = new Mock<IExtensibleModelBinder>();
            mockIntBinder
                .Expect(o => o.BindModel(controllerContext, It.IsAny<ExtensibleModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ExtensibleModelBindingContext mbc) {
                        mbc.Model = 42;
                        return true;
                    });
            bindingContext.ModelBinderProviders.RegisterBinderForType(typeof(int), mockIntBinder.Object, true /* suppressPrefixCheck */);

            KeyValuePairModelBinder<int, string> binder = new KeyValuePairModelBinder<int, string>();

            // Act
            bool retVal = binder.BindModel(controllerContext, bindingContext);

            // Assert
            Assert.IsTrue(retVal, "Want to return 'True' since the key was bound correctly and we want to push validation up.");
            Assert.IsNull(bindingContext.Model);
            CollectionAssert.AreEquivalent(new string[] { "someName.key" },
                bindingContext.ValidationNode.ChildNodes.Select(n => n.ModelStateKey).ToArray());
        }

        [TestMethod]
        public void BindModel_SubBindingSucceeds() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(KeyValuePair<int, string>)),
                ModelName = "someName",
                ModelBinderProviders = new ModelBinderProviderCollection(),
                ValueProvider = new SimpleValueProvider()
            };

            Mock<IExtensibleModelBinder> mockIntBinder = new Mock<IExtensibleModelBinder>();
            mockIntBinder
                .Expect(o => o.BindModel(controllerContext, It.IsAny<ExtensibleModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ExtensibleModelBindingContext mbc) {
                        mbc.Model = 42;
                        return true;
                    });
            bindingContext.ModelBinderProviders.RegisterBinderForType(typeof(int), mockIntBinder.Object, true /* suppressPrefixCheck */);
            Mock<IExtensibleModelBinder> mockStringBinder = new Mock<IExtensibleModelBinder>();
            mockStringBinder
                .Expect(o => o.BindModel(controllerContext, It.IsAny<ExtensibleModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ExtensibleModelBindingContext mbc) {
                        mbc.Model = "forty-two";
                        return true;
                    });
            bindingContext.ModelBinderProviders.RegisterBinderForType(typeof(string), mockStringBinder.Object, true /* suppressPrefixCheck */);

            KeyValuePairModelBinder<int, string> binder = new KeyValuePairModelBinder<int, string>();

            // Act
            bool retVal = binder.BindModel(controllerContext, bindingContext);

            // Assert
            Assert.IsTrue(retVal);
            Assert.AreEqual(new KeyValuePair<int, string>(42, "forty-two"), bindingContext.Model);
            CollectionAssert.AreEquivalent(new string[] { "someName.key", "someName.value" },
                bindingContext.ValidationNode.ChildNodes.Select(n => n.ModelStateKey).ToArray());
        }

    }
}
