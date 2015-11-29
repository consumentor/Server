namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Globalization;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ModelBinderConfigTest {

        [TestMethod]
        public void GetUserResourceString_NullControllerContext_ReturnsNull() {
            // Act
            string customResourceString = ModelBinderConfig.GetUserResourceString(null /* controllerContext */, "someResourceName", "someResourceClassKey");

            // Assert
            Assert.IsNull(customResourceString);
        }

        [TestMethod]
        public void GetUserResourceString_NullHttpContext_ReturnsNull() {
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(o => o.HttpContext).Returns((HttpContextBase)null);

            // Act
            string customResourceString = ModelBinderConfig.GetUserResourceString(mockControllerContext.Object, "someResourceName", "someResourceClassKey");

            // Assert
            Assert.IsNull(customResourceString);
        }

        [TestMethod]
        public void GetUserResourceString_NullResourceKey_ReturnsNull() {
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(o => o.HttpContext).Never();

            // Act
            string customResourceString = ModelBinderConfig.GetUserResourceString(mockControllerContext.Object, "someResourceName", null /* resourceClassKey */);

            // Assert
            Assert.IsNull(customResourceString);
        }

        [TestMethod]
        public void GetUserResourceString_ValidResourceObject_ReturnsResourceString() {
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(o => o.HttpContext.GetGlobalResourceObject("someResourceClassKey", "someResourceName", CultureInfo.CurrentUICulture)).Returns("My custom resource string");

            // Act
            string customResourceString = ModelBinderConfig.GetUserResourceString(mockControllerContext.Object, "someResourceName", "someResourceClassKey");

            // Assert
            Assert.AreEqual("My custom resource string", customResourceString);
        }

        [TestMethod]
        public void Initialize_ReplacesOriginalCollection() {
            // Arrange
            ModelBinderDictionary oldBinders = new ModelBinderDictionary();
            oldBinders[typeof(int)] = new Mock<IModelBinder>().Object;
            ModelBinderProviderCollection newBinderProviders = new ModelBinderProviderCollection();

            // Act
            ModelBinderConfig.Initialize(oldBinders, newBinderProviders);

            // Assert
            Assert.AreEqual(0, oldBinders.Count, "Old binder dictionary should have been cleared.");

            ExtensibleModelBinderAdapter shimBinder = oldBinders.DefaultBinder as ExtensibleModelBinderAdapter;
            Assert.IsNotNull(shimBinder, "The default binder for the old system should have been replaced with a compatibility shim.");
            Assert.AreSame(newBinderProviders, shimBinder.Providers, "Providers collection was not passed through correctly.");
        }

        [TestMethod]
        public void TypeConversionErrorMessageProvider_DefaultValue() {
            // Arrange
            ModelMetadata metadata = new ModelMetadata(new Mock<ModelMetadataProvider>().Object, null, null, typeof(int), "SomePropertyName");

            // Act
            string errorString = ModelBinderConfig.TypeConversionErrorMessageProvider(null, metadata, "some incoming value");

            // Assert
            Assert.AreEqual("The value 'some incoming value' is not valid for SomePropertyName.", errorString);
        }

        [TestMethod]
        public void TypeConversionErrorMessageProvider_Property() {
            // Arrange
            ModelBinderConfigWrapper wrapper = new ModelBinderConfigWrapper();

            // Act & assert
            try {
                MemberHelper.TestPropertyWithDefaultInstance(wrapper, "TypeConversionErrorMessageProvider", (ModelBinderErrorMessageProvider)DummyErrorSelector);
            }
            finally {
                wrapper.Reset();
            }
        }

        [TestMethod]
        public void ValueRequiredErrorMessageProvider_DefaultValue() {
            // Arrange
            ModelMetadata metadata = new ModelMetadata(new Mock<ModelMetadataProvider>().Object, null, null, typeof(int), "SomePropertyName");

            // Act
            string errorString = ModelBinderConfig.ValueRequiredErrorMessageProvider(null, metadata, "some incoming value");

            // Assert
            Assert.AreEqual("A value is required.", errorString);
        }

        [TestMethod]
        public void ValueRequiredErrorMessageProvider_Property() {
            // Arrange
            ModelBinderConfigWrapper wrapper = new ModelBinderConfigWrapper();

            // Act & assert
            try {
                MemberHelper.TestPropertyWithDefaultInstance(wrapper, "ValueRequiredErrorMessageProvider", (ModelBinderErrorMessageProvider)DummyErrorSelector);
            }
            finally {
                wrapper.Reset();
            }
        }

        private string DummyErrorSelector(ControllerContext controllerContext, ModelMetadata modelMetadata, object incomingValue) {
            throw new NotImplementedException();
        }

        private sealed class ModelBinderConfigWrapper {
            public ModelBinderErrorMessageProvider TypeConversionErrorMessageProvider {
                get {
                    return ModelBinderConfig.TypeConversionErrorMessageProvider;
                }
                set {
                    ModelBinderConfig.TypeConversionErrorMessageProvider = value;
                }
            }

            public ModelBinderErrorMessageProvider ValueRequiredErrorMessageProvider {
                get {
                    return ModelBinderConfig.ValueRequiredErrorMessageProvider;
                }
                set {
                    ModelBinderConfig.ValueRequiredErrorMessageProvider = value;
                }
            }

            public void Reset() {
                ModelBinderConfig.TypeConversionErrorMessageProvider = null;
                ModelBinderConfig.ValueRequiredErrorMessageProvider = null;
            }
        }

    }
}
