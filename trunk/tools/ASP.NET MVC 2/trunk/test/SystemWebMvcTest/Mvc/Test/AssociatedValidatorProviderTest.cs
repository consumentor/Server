namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AssociatedValidatorProviderTest {

        [TestMethod]
        public void GetValidatorsGuardClauses() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(object));
            Mock<AssociatedValidatorProvider> provider = new Mock<AssociatedValidatorProvider> { CallBase = true };

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => provider.Object.GetValidators(null, new ControllerContext()),
                "metadata");
            ExceptionHelper.ExpectArgumentNullException(
                () => provider.Object.GetValidators(metadata, null),
                "context");
        }

        [TestMethod]
        public void GetValidatorsForPropertyWithLocalAttributes() {
            // Arrange
            IEnumerable<Attribute> callbackAttributes = null;
            ControllerContext context = new ControllerContext();
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(PropertyModel), "LocalAttributes");
            Mock<TestableAssociatedValidatorProvider> provider = new Mock<TestableAssociatedValidatorProvider> { CallBase = true };
            provider.Expect(p => p.AbstractGetValidators(metadata, context, It.IsAny<IEnumerable<Attribute>>()))
                    .Callback<ModelMetadata, ControllerContext, IEnumerable<Attribute>>((m, c, attributes) => callbackAttributes = attributes)
                    .Returns(() => null)
                    .Verifiable();

            // Act
            provider.Object.GetValidators(metadata, context);

            // Assert
            provider.Verify();
            Assert.IsTrue(callbackAttributes.Any(a => a is RequiredAttribute));
        }

        [TestMethod]
        public void GetValidatorsForPropertyWithMetadataAttributes() {
            // Arrange
            IEnumerable<Attribute> callbackAttributes = null;
            ControllerContext context = new ControllerContext();
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(PropertyModel), "MetadataAttributes");
            Mock<TestableAssociatedValidatorProvider> provider = new Mock<TestableAssociatedValidatorProvider> { CallBase = true };
            provider.Expect(p => p.AbstractGetValidators(metadata, context, It.IsAny<IEnumerable<Attribute>>()))
                    .Callback<ModelMetadata, ControllerContext, IEnumerable<Attribute>>((m, c, attributes) => callbackAttributes = attributes)
                    .Returns(() => null)
                    .Verifiable();

            // Act
            provider.Object.GetValidators(metadata, context);

            // Assert
            provider.Verify();
            Assert.IsTrue(callbackAttributes.Any(a => a is RangeAttribute));
        }

        [TestMethod]
        public void GetValidatorsForPropertyWithMixedAttributes() {
            // Arrange
            IEnumerable<Attribute> callbackAttributes = null;
            ControllerContext context = new ControllerContext();
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(PropertyModel), "MixedAttributes");
            Mock<TestableAssociatedValidatorProvider> provider = new Mock<TestableAssociatedValidatorProvider> { CallBase = true };
            provider.Expect(p => p.AbstractGetValidators(metadata, context, It.IsAny<IEnumerable<Attribute>>()))
                    .Callback<ModelMetadata, ControllerContext, IEnumerable<Attribute>>((m, c, attributes) => callbackAttributes = attributes)
                    .Returns(() => null)
                    .Verifiable();

            // Act
            provider.Object.GetValidators(metadata, context);

            // Assert
            provider.Verify();
            Assert.IsTrue(callbackAttributes.Any(a => a is RangeAttribute));
            Assert.IsTrue(callbackAttributes.Any(a => a is RequiredAttribute));
        }

        [MetadataType(typeof(PropertyModel.Metadata))]
        private class PropertyModel {
            [Required]
            public int LocalAttributes { get; set; }

            public string MetadataAttributes { get; set; }

            [Required]
            public double MixedAttributes { get; set; }

            private class Metadata {
                [Range(10, 100)]
                public object MetadataAttributes { get; set; }

                [Range(10, 100)]
                public object MixedAttributes { get; set; }
            }
        }

        public abstract class TestableAssociatedValidatorProvider : AssociatedValidatorProvider {
            protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context, IEnumerable<Attribute> attributes) {
                return AbstractGetValidators(metadata, context, attributes);
            }

            // Hoist access
            public abstract IEnumerable<ModelValidator> AbstractGetValidators(ModelMetadata metadata, ControllerContext context, IEnumerable<Attribute> attributes);
        }

    }
}
