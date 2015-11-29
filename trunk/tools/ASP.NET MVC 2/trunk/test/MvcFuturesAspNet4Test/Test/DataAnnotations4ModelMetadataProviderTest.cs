namespace Microsoft.Web.Mvc.AspNet4.Test {
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataAnnotations4ModelMetadataProviderTest {

        // DisplayAttribute tests

        class DataTypeModel {
            public int NoAttribute { get; set; }

            [DisplayFormat(HtmlEncode = true)]
            public int HtmlEncodeTrue { get; set; }

            [DisplayFormat(HtmlEncode = false)]
            public int HtmlEncodeFalse { get; set; }

            [DataType(DataType.Currency)]
            [DisplayFormat(HtmlEncode = false)]
            public int HtmlEncodeFalseWithDataType { get; set; }    // DataType trumps DisplayFormat.HtmlEncode
        }

        [TestMethod]
        public void DataTypeInfluencedByDisplayFormatAttributeHtmlEncode() {
            // Arrange
            DataAnnotations4ModelMetadataProvider provider = new DataAnnotations4ModelMetadataProvider();

            // Act & Assert
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DataTypeModel), "NoAttribute").DataTypeName);
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DataTypeModel), "HtmlEncodeTrue").DataTypeName);
            Assert.AreEqual("Html", provider.GetMetadataForProperty(null, typeof(DataTypeModel), "HtmlEncodeFalse").DataTypeName);
            Assert.AreEqual("Currency", provider.GetMetadataForProperty(null, typeof(DataTypeModel), "HtmlEncodeFalseWithDataType").DataTypeName);
        }

        class DescriptionModel {
            public int NoAttribute { get; set; }

            [Display]
            public int DescriptionNotSet { get; set; }

            [Display(Description = "Description text")]
            public int DescriptionSet { get; set; }
        }

        [TestMethod]
        public void DescriptionTests() {
            // Arrange
            DataAnnotations4ModelMetadataProvider provider = new DataAnnotations4ModelMetadataProvider();

            // Act & Assert
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DescriptionModel), "NoAttribute").Description);
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DescriptionModel), "DescriptionNotSet").Description);
            Assert.AreEqual("Description text", provider.GetMetadataForProperty(null, typeof(DescriptionModel), "DescriptionSet").Description);
        }

        class DisplayNameModel {
            public int NoAttribute { get; set; }

            [Display]
            public int DisplayAttributeNameNotSet { get; set; }

            [Display(Name = "Non empty name")]
            public int DisplayAttributeNonEmptyName { get; set; }

            [Display]
            [DisplayName("Value from DisplayName")]
            public int BothAttributesNameNotSet { get; set; }

            [Display(Name = "Value from Display")]
            [DisplayName("Value from DisplayName")]
            public int BothAttributes { get; set; }    // Display trumps DisplayName
        }

        [TestMethod]
        public void DisplayNameTests() {
            // Arrange
            DataAnnotations4ModelMetadataProvider provider = new DataAnnotations4ModelMetadataProvider();

            // Act & Assert
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DisplayNameModel), "NoAttribute").DisplayName);
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DisplayNameModel), "DisplayAttributeNameNotSet").DisplayName);
            Assert.AreEqual("Non empty name", provider.GetMetadataForProperty(null, typeof(DisplayNameModel), "DisplayAttributeNonEmptyName").DisplayName);
            Assert.AreEqual("Value from DisplayName", provider.GetMetadataForProperty(null, typeof(DisplayNameModel), "BothAttributesNameNotSet").DisplayName);
            Assert.AreEqual("Value from Display", provider.GetMetadataForProperty(null, typeof(DisplayNameModel), "BothAttributes").DisplayName);
        }

        class ShortDisplayNameModel {
            public int NoAttribute { get; set; }

            [Display]
            public int ShortNameNotSet { get; set; }

            [Display(ShortName = "Short name")]
            public int ShortNameSet { get; set; }
        }

        [TestMethod]
        public void ShortDisplayNameTests() {
            // Arrange
            DataAnnotations4ModelMetadataProvider provider = new DataAnnotations4ModelMetadataProvider();

            // Act & Assert
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(ShortDisplayNameModel), "NoAttribute").ShortDisplayName);
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(ShortDisplayNameModel), "ShortNameNotSet").ShortDisplayName);
            Assert.AreEqual("Short name", provider.GetMetadataForProperty(null, typeof(ShortDisplayNameModel), "ShortNameSet").ShortDisplayName);
        }

        class WatermarkModel {
            public int NoAttribute { get; set; }

            [Display]
            public int PromptNotSet { get; set; }

            [Display(Prompt = "Enter stuff here")]
            public int PromptSet { get; set; }
        }

        [TestMethod]
        public void WatermarkTests() {
            // Arrange
            DataAnnotations4ModelMetadataProvider provider = new DataAnnotations4ModelMetadataProvider();

            // Act & Assert
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(WatermarkModel), "NoAttribute").Watermark);
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(WatermarkModel), "PromptNotSet").Watermark);
            Assert.AreEqual("Enter stuff here", provider.GetMetadataForProperty(null, typeof(WatermarkModel), "PromptSet").Watermark);
        }

        // EditableAttribute tests

        class ReadOnlyModel {
            public int NoAttributes { get; set; }

            [ReadOnly(true)]
            public int ReadOnlyAttribute { get; set; }

            [Editable(false)]
            public int EditableAttribute { get; set; }

            [ReadOnly(true)]
            [Editable(true)]
            public int BothAttributes { get; set; }    // Editable trumps ReadOnly
        }

        [TestMethod]
        public void ReadOnlyTests() {
            // Arrange
            DataAnnotations4ModelMetadataProvider provider = new DataAnnotations4ModelMetadataProvider();

            // Act & Assert
            Assert.IsFalse(provider.GetMetadataForProperty(null, typeof(ReadOnlyModel), "NoAttributes").IsReadOnly);
            Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(ReadOnlyModel), "ReadOnlyAttribute").IsReadOnly);
            Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(ReadOnlyModel), "EditableAttribute").IsReadOnly);
            Assert.IsFalse(provider.GetMetadataForProperty(null, typeof(ReadOnlyModel), "BothAttributes").IsReadOnly);
        }

        // IMetadataAware tests

        [AdditionalMetadata("ClassName", "ClassValue")]
        class ClassWithAdditionalMetadata {
            [AdditionalMetadata("PropertyName", "PropertyValue")]
            public int MyProperty { get; set; }
        }

        [TestMethod]
        public void MetadataAwareAttributeCanModifyTypeMetadata() {
            // Arrange
            DataAnnotations4ModelMetadataProvider provider = new DataAnnotations4ModelMetadataProvider();

            // Act
            ModelMetadata metadata = provider.GetMetadataForType(null, typeof(ClassWithAdditionalMetadata));

            // Assert
            var kvp = metadata.AdditionalValues.Single();
            Assert.AreEqual("ClassName", kvp.Key);
            Assert.AreEqual("ClassValue", kvp.Value);
        }

        [TestMethod]
        public void MetadataAwareAttributeCanModifyPropertyMetadata() {
            // Arrange
            DataAnnotations4ModelMetadataProvider provider = new DataAnnotations4ModelMetadataProvider();

            // Act
            ModelMetadata metadata = provider.GetMetadataForProperty(null, typeof(ClassWithAdditionalMetadata), "MyProperty");

            // Assert
            var kvp = metadata.AdditionalValues.Single();
            Assert.AreEqual("PropertyName", kvp.Key);
            Assert.AreEqual("PropertyValue", kvp.Value);
        }

    }
}
