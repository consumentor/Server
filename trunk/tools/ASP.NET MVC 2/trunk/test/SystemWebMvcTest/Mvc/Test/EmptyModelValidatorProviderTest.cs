namespace System.Web.Mvc.Test {
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EmptyModelValidatorProviderTest {

        [TestMethod]
        public void ReturnsNoValidators() {
            // Arrange
            EmptyModelValidatorProvider provider = new EmptyModelValidatorProvider();

            // Act
            IEnumerable<ModelValidator> result = provider.GetValidators(null, null);

            // Assert
            Assert.IsFalse(result.Any());
        }

    }
}
