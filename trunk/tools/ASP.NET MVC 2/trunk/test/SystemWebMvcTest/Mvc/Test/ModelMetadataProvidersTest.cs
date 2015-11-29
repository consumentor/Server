namespace System.Web.Mvc.Test {
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelMetadataProvidersTest {
        [TestMethod]
        public void DefaultModelMetadataProviderIsDataAnnotations() {
            // Act
            ModelMetadataProvider provider = ModelMetadataProviders.Current;

            // Assert
            Assert.AreEqual(typeof(DataAnnotationsModelMetadataProvider), provider.GetType());
        }

        [TestMethod]
        public void SettingNullModelMetadataProviderUsesEmptyModelMetadataProvider() {
            ModelMetadataProvider original = ModelMetadataProviders.Current;

            try {
                // Act
                ModelMetadataProviders.Current = null;

                // Assert
                Assert.AreEqual(typeof(EmptyModelMetadataProvider), ModelMetadataProviders.Current.GetType());
            }
            finally {
                ModelMetadataProviders.Current = original;
            }
        }
    }
}
