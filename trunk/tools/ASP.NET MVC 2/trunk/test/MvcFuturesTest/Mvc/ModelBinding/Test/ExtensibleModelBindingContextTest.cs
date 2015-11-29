namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class ExtensibleModelBindingContextTest {

        [TestMethod]
        public void CopyConstructor() {
            // Arrange
            ExtensibleModelBindingContext originalBindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(object)),
                ModelName = "theName",
                ModelState = new ModelStateDictionary(),
                ValueProvider = new SimpleValueProvider()
            };

            // Act
            ExtensibleModelBindingContext newBindingContext = new ExtensibleModelBindingContext(originalBindingContext);

            // Assert
            Assert.IsNull(newBindingContext.ModelMetadata, "Property 'ModelMetadata' should not have been propagated.");
            Assert.AreEqual("", newBindingContext.ModelName, "Property 'ModelName' should not have been propagated.");
            Assert.AreEqual(originalBindingContext.ModelState, newBindingContext.ModelState, "Property 'ModelState' should have been propagated.");
            Assert.AreEqual(originalBindingContext.ValueProvider, newBindingContext.ValueProvider, "Property 'ValueProvider' should have been propagated.");
        }

        [TestMethod]
        public void ModelBinderProvidersProperty() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext();

            // Act & assert
            MemberHelper.TestPropertyWithDefaultInstance(bindingContext, "ModelBinderProviders", new ModelBinderProviderCollection(), ModelBinderProviders.Providers);
        }

        [TestMethod]
        public void ModelProperty() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(int))
            };

            // Act & assert
            MemberHelper.TestPropertyValue(bindingContext, "Model", 42);
        }

        [TestMethod]
        public void ModelProperty_ThrowsIfModelMetadataDoesNotExist() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext();

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    bindingContext.Model = null;
                },
                "The ModelMetadata property must be set before accessing this property.");
        }

        [TestMethod]
        public void ModelNameProperty() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext();

            // Act & assert
            MemberHelper.TestStringProperty(bindingContext, "ModelName", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

        [TestMethod]
        public void ModelStateProperty() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext();
            ModelStateDictionary modelState = new ModelStateDictionary();

            // Act & assert
            MemberHelper.TestPropertyWithDefaultInstance(bindingContext, "ModelState", modelState);
        }

        [TestMethod]
        public void ModelAndModelTypeAreFedFromModelMetadata() {
            // Act
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => 42, typeof(int))
            };

            // Assert
            Assert.AreEqual(42, bindingContext.Model);
            Assert.AreEqual(typeof(int), bindingContext.ModelType);
        }

        [TestMethod]
        public void ValidationNodeProperty() {
            // Act
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => 42, typeof(int))
            };

            // Act & assert
            MemberHelper.TestPropertyWithDefaultInstance(bindingContext, "ValidationNode", new ModelValidationNode(bindingContext.ModelMetadata, "someName"));
        }

        [TestMethod]
        public void ValidationNodeProperty_DefaultValues() {
            // Act
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => 42, typeof(int)),
                ModelName = "theInt"
            };

            // Act
            ModelValidationNode validationNode = bindingContext.ValidationNode;

            // Assert
            Assert.IsNotNull(validationNode);
            Assert.AreEqual(bindingContext.ModelMetadata, validationNode.ModelMetadata);
            Assert.AreEqual(bindingContext.ModelName, validationNode.ModelStateKey);
        }

    }
}
