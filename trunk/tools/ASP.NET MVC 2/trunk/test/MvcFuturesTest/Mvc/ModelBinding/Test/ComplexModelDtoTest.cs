namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ComplexModelDtoTest {

        [TestMethod]
        public void ConstructorThrowsIfModelMetadataIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ComplexModelDto(null, Enumerable.Empty<ModelMetadata>());
                }, "modelMetadata");
        }

        [TestMethod]
        public void ConstructorThrowsIfPropertyMetadataIsNull() {
            // Arrange
            ModelMetadata modelMetadata = GetModelMetadata();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ComplexModelDto(modelMetadata, null);
                }, "propertyMetadata");
        }

        [TestMethod]
        public void ConstructorSetsProperties() {
            // Arrange
            ModelMetadata modelMetadata = GetModelMetadata();
            ModelMetadata[] propertyMetadata = new ModelMetadata[0];

            // Act
            ComplexModelDto dto = new ComplexModelDto(modelMetadata, propertyMetadata);

            // Assert
            Assert.AreEqual(modelMetadata, dto.ModelMetadata);
            CollectionAssert.AreEqual(propertyMetadata, dto.PropertyMetadata);
            Assert.AreEqual(0, dto.Results.Count, "Results dictionary should have been empty.");
        }

        private static ModelMetadata GetModelMetadata() {
            return new ModelMetadata(new EmptyModelMetadataProvider(), typeof(object), null, typeof(object), "PropertyName");
        }

    }
}
