namespace Microsoft.Web.Mvc.AspNet4.Test {
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class DataAnnotations4ModelValidatorTest {

        [TestMethod]
        public void ConstructorGuards() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(object));
            ControllerContext context = new ControllerContext();
            RequiredAttribute attribute = new RequiredAttribute();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => new DataAnnotations4ModelValidator(null, context, attribute),
                "metadata");
            ExceptionHelper.ExpectArgumentNullException(
                () => new DataAnnotations4ModelValidator(metadata, null, attribute),
                "controllerContext");
            ExceptionHelper.ExpectArgumentNullException(
                () => new DataAnnotations4ModelValidator(metadata, context, null),
                "attribute");
        }

        [TestMethod]
        public void ValuesSet() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => 15, typeof(string), "Length");
            ControllerContext context = new ControllerContext();
            RequiredAttribute attribute = new RequiredAttribute();

            // Act
            DataAnnotations4ModelValidator validator = new DataAnnotations4ModelValidator(metadata, context, attribute);

            // Assert
            Assert.AreSame(attribute, validator.Attribute);
            Assert.AreEqual(attribute.FormatErrorMessage("Length"), validator.ErrorMessage);
        }

        [TestMethod]
        public void NoClientRulesByDefault() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => 15, typeof(string), "Length");
            ControllerContext context = new ControllerContext();
            RequiredAttribute attribute = new RequiredAttribute();

            // Act
            DataAnnotations4ModelValidator validator = new DataAnnotations4ModelValidator(metadata, context, attribute);

            // Assert
            Assert.IsFalse(validator.GetClientValidationRules().Any());
        }

        [TestMethod]
        public void ValidateWithIsValidTrue() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => 15, typeof(string), "Length");
            ControllerContext context = new ControllerContext();
            Mock<ValidationAttribute> attribute = new Mock<ValidationAttribute> { CallBase = true };
            attribute.Expect(a => a.IsValid(metadata.Model)).Returns(true);
            DataAnnotations4ModelValidator validator = new DataAnnotations4ModelValidator(metadata, context, attribute.Object);

            // Act
            IEnumerable<ModelValidationResult> result = validator.Validate(null);

            // Assert
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void ValidateWithIsValidFalse() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => 15, typeof(string), "Length");
            ControllerContext context = new ControllerContext();
            Mock<ValidationAttribute> attribute = new Mock<ValidationAttribute> { CallBase = true };
            attribute.Expect(a => a.IsValid(metadata.Model)).Returns(false);
            DataAnnotations4ModelValidator validator = new DataAnnotations4ModelValidator(metadata, context, attribute.Object);

            // Act
            IEnumerable<ModelValidationResult> result = validator.Validate(null);

            // Assert
            var validationResult = result.Single();
            Assert.AreEqual("", validationResult.MemberName);
            Assert.AreEqual(attribute.Object.FormatErrorMessage("Length"), validationResult.Message);
        }

        [TestMethod]
        public void AttributeWithIClientValidatableGetsClientValidationRules() {
            // Arrange
            var expected = new ModelClientValidationStringLengthRule("Error", 1, 10);
            var context = new ControllerContext();
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, typeof(string));
            var attribute = new Mock<ValidationAttribute> { CallBase = true };
            attribute.As<IClientValidatable>()
                     .Expect(cv => cv.GetClientValidationRules(metadata, context))
                     .Returns(new[] { expected })
                     .Verifiable();
            var validator = new DataAnnotations4ModelValidator(metadata, context, attribute.Object);

            // Act
            ModelClientValidationRule actual = validator.GetClientValidationRules().Single();

            // Assert
            attribute.Verify();
            Assert.AreSame(expected, actual);
        }

    }
}
