namespace System.Web.Mvc.Test {
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ModelValidatorProviderCollectionTest {

        [TestMethod]
        public void ListWrappingConstructor() {
            // Arrange
            List<ModelValidatorProvider> list = new List<ModelValidatorProvider>() {
                new Mock<ModelValidatorProvider>().Object, new Mock<ModelValidatorProvider>().Object 
            };

            // Act
            ModelValidatorProviderCollection collection = new ModelValidatorProviderCollection(list);

            // Assert
            CollectionAssert.AreEqual(list, collection);
        }

        [TestMethod]
        public void ListWrappingConstructorThrowsIfListIsNull() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ModelValidatorProviderCollection(null);
                },
                "list");
        }

        [TestMethod]
        public void DefaultConstructor() {
            // Act
            ModelValidatorProviderCollection collection = new ModelValidatorProviderCollection();

            // Assert
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddNullModelValidatorProviderThrows() {
            // Arrange
            ModelValidatorProviderCollection collection = new ModelValidatorProviderCollection();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    collection.Add(null);
                },
                "item");
        }

        [TestMethod]
        public void SetItem() {
            // Arrange
            ModelValidatorProviderCollection collection = new ModelValidatorProviderCollection();
            collection.Add(new Mock<ModelValidatorProvider>().Object);

            ModelValidatorProvider newProvider = new Mock<ModelValidatorProvider>().Object;

            // Act
            collection[0] = newProvider;

            // Assert
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(newProvider, collection[0]);
        }

        [TestMethod]
        public void SetNullModelValidatorProviderThrows() {
            // Arrange
            ModelValidatorProviderCollection collection = new ModelValidatorProviderCollection();
            collection.Add(new Mock<ModelValidatorProvider>().Object);

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    collection[0] = null;
                },
                "item");
        }

        [TestMethod]
        public void GetValidators() {
            // Arrange
            ModelMetadata metadata = GetMetadata();
            ControllerContext controllerContext = new ControllerContext();

            SimpleModelValidator[] allValidators = new SimpleModelValidator[] {
                new SimpleModelValidator(),
                new SimpleModelValidator(),
                new SimpleModelValidator(),
                new SimpleModelValidator(),
                new SimpleModelValidator()
            };

            Mock<ModelValidatorProvider> provider1 = new Mock<ModelValidatorProvider>();
            provider1.Expect(p => p.GetValidators(metadata, controllerContext)).Returns(new ModelValidator[] {
                allValidators[0], allValidators[1]
            });

            Mock<ModelValidatorProvider> provider2 = new Mock<ModelValidatorProvider>();
            provider2.Expect(p => p.GetValidators(metadata, controllerContext)).Returns(new ModelValidator[] {
                allValidators[2], allValidators[3], allValidators[4]
            });

            ModelValidatorProviderCollection collection = new ModelValidatorProviderCollection();
            collection.Add(provider1.Object);
            collection.Add(provider2.Object);

            // Act
            IEnumerable<ModelValidator> returnedValidators = collection.GetValidators(metadata, controllerContext);

            // Assert
            CollectionAssert.AreEqual(allValidators, returnedValidators.ToArray());
        }

        private static ModelMetadata GetMetadata() {
            ModelMetadataProvider provider = new EmptyModelMetadataProvider();
            return provider.GetMetadataForType(null, typeof(object));
        }

        private sealed class SimpleModelValidator : ModelValidator {
            public SimpleModelValidator()
                : base(GetMetadata(), new ControllerContext()) {
            }
            public override IEnumerable<ModelValidationResult> Validate(object container) {
                throw new NotImplementedException();
            }
        }

    }
}
