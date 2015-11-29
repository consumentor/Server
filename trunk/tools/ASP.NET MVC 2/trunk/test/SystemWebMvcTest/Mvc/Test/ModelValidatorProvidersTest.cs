namespace System.Web.Mvc.Test {
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelValidatorProvidersTest {

        [TestMethod]
        public void CollectionDefaults() {
            // Arrange
            Type[] expectedTypes = new Type[] {
                typeof(DataAnnotationsModelValidatorProvider),
                typeof(DataErrorInfoModelValidatorProvider),
                typeof(ClientDataTypeModelValidatorProvider)
            };

            // Act
            Type[] actualTypes = ModelValidatorProviders.Providers.Select(p => p.GetType()).ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedTypes, actualTypes);
        }

    }
}
