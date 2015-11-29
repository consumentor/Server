namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelBinderProvidersTest {

        [TestMethod]
        public void CollectionDefaults() {
            // Arrange
            Type[] expectedTypes = new Type[] {
                typeof(TypeMatchModelBinderProvider),
                typeof(BinaryDataModelBinderProvider),
                typeof(KeyValuePairModelBinderProvider),
                typeof(ComplexModelDtoModelBinderProvider),
                typeof(ArrayModelBinderProvider),
                typeof(DictionaryModelBinderProvider),
                typeof(CollectionModelBinderProvider),
                typeof(TypeConverterModelBinderProvider),
                typeof(MutableObjectModelBinderProvider)
            };

            // Act
            Type[] actualTypes = ModelBinderProviders.Providers.Select(p => p.GetType()).ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedTypes, actualTypes);
        }

    }
}
