namespace System.Web.Mvc.Test {
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ValueProviderFactoriesTest {

        [TestMethod]
        public void CollectionDefaults() {
            // Arrange
            Type[] expectedTypes = new Type[] {
                typeof(FormValueProviderFactory),
                typeof(RouteDataValueProviderFactory),
                typeof(QueryStringValueProviderFactory),
                typeof(HttpFileCollectionValueProviderFactory)
            };

            // Act
            Type[] actualTypes = ValueProviderFactories.Factories.Select(p => p.GetType()).ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedTypes, actualTypes);
        }

    }
}
