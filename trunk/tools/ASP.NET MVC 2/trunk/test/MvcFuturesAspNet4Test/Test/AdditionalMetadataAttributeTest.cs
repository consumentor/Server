namespace Microsoft.Web.Mvc.AspNet4 {
    using System;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AdditionalMetadataAttributeTest {

        [TestMethod]
        public void NameValueProperties() {
            // Act
            AdditionalMetadataAttribute attr = new AdditionalMetadataAttribute("someName", "someValue");

            // Assert
            Assert.AreEqual("someName", attr.Name);
            Assert.AreEqual("someValue", attr.Value);
        }

        [TestMethod]
        public void OnMetadataCreated_SetsAdditionalValuesKey() {
            // Arrange
            ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(object));
            metadata.AdditionalValues["someKey"] = "oldValue";

            AdditionalMetadataAttribute attr = new AdditionalMetadataAttribute("someKey", "newValue");

            // Act
            ((IMetadataAware)attr).OnMetadataCreated(metadata);

            // Assert
            Assert.AreEqual("newValue", metadata.AdditionalValues["someKey"]);
        }

        [TestMethod]
        public void TypeIdProperty() {
            // Arrange
            AdditionalMetadataAttribute attrFoo1 = new AdditionalMetadataAttribute("foo", "fooValue1");
            AdditionalMetadataAttribute attrFoo2 = new AdditionalMetadataAttribute("Foo", "fooValue2");
            AdditionalMetadataAttribute attrBar = new AdditionalMetadataAttribute("bar", "barValue1");

            // Act & assert
            Assert.AreEqual(attrFoo1.TypeId.GetHashCode(), attrFoo2.TypeId.GetHashCode(), "TypeId hash codes should match - case insensitive comparison.");
            Assert.AreEqual(attrFoo1.TypeId, attrFoo2.TypeId, "TypeId values should match - case insentivie comparison.");
            Assert.AreNotEqual(attrFoo1.TypeId, attrBar.TypeId, "TypeId values should not match between 'foo' and 'bar' keys.");
        }

    }
}
