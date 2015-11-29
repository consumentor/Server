namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ComplexModelDtoResultTest {

        [TestMethod]
        public void Constructor_ThrowsIfValidationNodeIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ComplexModelDtoResult("some string", null);
                }, "validationNode");
        }

        [TestMethod]
        public void Constructor_SetsProperties() {
            // Arrange
            ModelValidationNode validationNode = GetValidationNode();

            // Act
            ComplexModelDtoResult result = new ComplexModelDtoResult("some string", validationNode);

            // Assert
            Assert.AreEqual("some string", result.Model);
            Assert.AreEqual(validationNode, result.ValidationNode);
        }

        private static ModelValidationNode GetValidationNode() {
            EmptyModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelMetadata metadata = provider.GetMetadataForType(null, typeof(object));
            return new ModelValidationNode(metadata, "someKey");
        }

    }
}
