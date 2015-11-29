namespace Microsoft.Web.Mvc.AspNet4.Test {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ValidatableObjectAdapterTest {
        // IValidatableObject support

        [TestMethod]
        public void NonIValidatableObjectInsideMetadataThrows() {
            // Arrange
            var context = new ControllerContext();
            var validatable = new Mock<IValidatableObject>();
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => 42, typeof(IValidatableObject));
            var validator = new ValidatableObjectAdapter(metadata, context);

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => validator.Validate(null),
                "The model object inside the metadata claimed to be compatible with System.ComponentModel.DataAnnotations.IValidatableObject, but was actually System.Int32.");
        }

        [TestMethod]
        public void IValidatableObjectGetsAProperlyPopulatedValidationContext() {
            // Arrange
            var context = new ControllerContext();
            var validatable = new Mock<IValidatableObject>();
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => validatable.Object, validatable.Object.GetType());
            var validator = new ValidatableObjectAdapter(metadata, context);
            ValidationContext validationContext = null;
            validatable.Expect(vo => vo.Validate(It.IsAny<ValidationContext>()))
                       .Callback<ValidationContext>(vc => validationContext = vc)
                       .Returns(Enumerable.Empty<ValidationResult>())
                       .Verifiable();

            // Act
            validator.Validate(null);

            // Assert
            validatable.Verify();
            Assert.AreSame(validatable.Object, validationContext.ObjectInstance);
            Assert.IsNull(validationContext.MemberName);
        }

        [TestMethod]
        public void IValidatableObjectWithNoErrors() {
            // Arrange
            var context = new ControllerContext();
            var validatable = new Mock<IValidatableObject>();
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => validatable.Object, validatable.Object.GetType());
            var validator = new ValidatableObjectAdapter(metadata, context);
            validatable.Expect(vo => vo.Validate(It.IsAny<ValidationContext>()))
                       .Returns(Enumerable.Empty<ValidationResult>());

            // Act
            IEnumerable<ModelValidationResult> results = validator.Validate(null);

            // Assert
            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public void IValidatableObjectWithModelLevelError() {
            // Arrange
            var context = new ControllerContext();
            var validatable = new Mock<IValidatableObject>();
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => validatable.Object, validatable.Object.GetType());
            var validator = new ValidatableObjectAdapter(metadata, context);
            validatable.Expect(vo => vo.Validate(It.IsAny<ValidationContext>()))
                       .Returns(new ValidationResult[] { new ValidationResult("Error Message") });

            // Act
            ModelValidationResult result = validator.Validate(null).Single();

            // Assert
            Assert.AreEqual("Error Message", result.Message);
            Assert.AreEqual(String.Empty, result.MemberName);
        }

        [TestMethod]
        public void IValidatableObjectWithMultipleModelLevelErrors() {
            // Arrange
            var context = new ControllerContext();
            var validatable = new Mock<IValidatableObject>();
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => validatable.Object, validatable.Object.GetType());
            var validator = new ValidatableObjectAdapter(metadata, context);
            validatable.Expect(vo => vo.Validate(It.IsAny<ValidationContext>()))
                       .Returns(new ValidationResult[] {
                           new ValidationResult("Error Message 1"),
                           new ValidationResult("Error Message 2")
                       });

            // Act
            ModelValidationResult[] results = validator.Validate(null).ToArray();

            // Assert
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual("Error Message 1", results[0].Message);
            Assert.AreEqual("Error Message 2", results[1].Message);
        }

        [TestMethod]
        public void IValidatableObjectWithMultiPropertyValidationFailure() {
            // Arrange
            var context = new ControllerContext();
            var validatable = new Mock<IValidatableObject>();
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => validatable.Object, validatable.Object.GetType());
            var validator = new ValidatableObjectAdapter(metadata, context);
            validatable.Expect(vo => vo.Validate(It.IsAny<ValidationContext>()))
                       .Returns(new[] { new ValidationResult("Error Message", new[] { "Property1", "Property2" }) })
                       .Verifiable();

            // Act
            ModelValidationResult[] results = validator.Validate(null).ToArray();

            // Assert
            validatable.Verify();
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual("Error Message", results[0].Message);
            Assert.AreEqual("Property1", results[0].MemberName);
            Assert.AreEqual("Error Message", results[1].Message);
            Assert.AreEqual("Property2", results[1].MemberName);
        }

        [TestMethod]
        public void IValidatableObjectWhichIsNullReturnsNoErrors() {
            // Arrange
            var context = new ControllerContext();
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, typeof(IValidatableObject));
            var validator = new ValidatableObjectAdapter(metadata, context);

            // Act
            IEnumerable<ModelValidationResult> results = validator.Validate(null);

            // Assert
            Assert.AreEqual(0, results.Count());
        }

    }
}
