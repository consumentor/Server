namespace System.Web.Mvc.Test {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataAnnotationsModelMetadataProviderTest {
        [TestMethod]
        public void InheritsAssociatedMetadataBehavior() {
            // Assert
            Assert.IsTrue(new DataAnnotationsModelMetadataProvider() is AssociatedMetadataProvider);
        }

        [TestMethod]
        public void GetMetadataForPropertiesSetTypesAndPropertyNames() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act
            IEnumerable<ModelMetadata> result = provider.GetMetadataForProperties("foo", typeof(string));

            // Assert
            Assert.IsTrue(result.Any(m => m.ModelType == typeof(int)
                                          && m.PropertyName == "Length"
                                          && (int)m.Model == 3));
        }

        [TestMethod]
        public void GetMetadataForPropertySetsTypeAndPropertyName() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act
            ModelMetadata result = provider.GetMetadataForProperty(null, typeof(string), "Length");

            // Assert
            Assert.AreEqual(typeof(int), result.ModelType);
            Assert.AreEqual("Length", result.PropertyName);
        }

        [TestMethod]
        public void GetMetadataForTypeSetsTypeWithNullPropertyName() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act
            ModelMetadata result = provider.GetMetadataForType(null, typeof(string));

            // Assert
            Assert.AreEqual(typeof(string), result.ModelType);
            Assert.IsNull(result.PropertyName);
        }

        class HiddenModel {
            public int NoAttribute { get; set; }

            [HiddenInput]
            public int DefaultHidden { get; set; }

            [HiddenInput(DisplayValue = false)]
            public int HiddenWithDisplayValueFalse { get; set; }

            [HiddenInput]
            [UIHint("CustomUIHint")]
            public int HiddenAndUIHint { get; set; }
        }

        [TestMethod]
        public void HiddenAttributeSetsTemplateHintAndHideSurroundingHtml() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act & Assert
            ModelMetadata noAttributeMetadata = provider.GetMetadataForProperty(null, typeof(HiddenModel), "NoAttribute");
            Assert.IsNull(noAttributeMetadata.TemplateHint);
            Assert.IsFalse(noAttributeMetadata.HideSurroundingHtml);

            ModelMetadata defaultHiddenMetadata = provider.GetMetadataForProperty(null, typeof(HiddenModel), "DefaultHidden");
            Assert.AreEqual("HiddenInput", defaultHiddenMetadata.TemplateHint);
            Assert.IsFalse(defaultHiddenMetadata.HideSurroundingHtml);

            ModelMetadata hiddenWithDisplayValueFalseMetadata = provider.GetMetadataForProperty(null, typeof(HiddenModel), "HiddenWithDisplayValueFalse");
            Assert.AreEqual("HiddenInput", hiddenWithDisplayValueFalseMetadata.TemplateHint);
            Assert.IsTrue(hiddenWithDisplayValueFalseMetadata.HideSurroundingHtml);

            // [UIHint] overrides the template hint from [Hidden]
            Assert.AreEqual("CustomUIHint", provider.GetMetadataForProperty(null, typeof(HiddenModel), "HiddenAndUIHint").TemplateHint);
        }

        class UIHintModel {
            public int NoAttribute { get; set; }

            [UIHint("MyCustomTemplate")]
            public int DefaultUIHint { get; set; }

            [UIHint("MyMvcTemplate", "MVC")]
            public int MvcUIHint { get; set; }

            [UIHint("MyWebFormsTemplate", "WebForms")]
            public int NoMvcUIHint { get; set; }

            [UIHint("MyDefaultTemplate")]
            [UIHint("MyWebFormsTemplate", "WebForms")]
            [UIHint("MyMvcTemplate", "MVC")]
            public int MultipleUIHint { get; set; }
        }

        [TestMethod]
        public void UIHintAttributeSetsTemplateHint() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act & Assert
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(UIHintModel), "NoAttribute").TemplateHint);
            Assert.AreEqual("MyCustomTemplate", provider.GetMetadataForProperty(null, typeof(UIHintModel), "DefaultUIHint").TemplateHint);
            Assert.AreEqual("MyMvcTemplate", provider.GetMetadataForProperty(null, typeof(UIHintModel), "MvcUIHint").TemplateHint);
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(UIHintModel), "NoMvcUIHint").TemplateHint);

#if false
            // This test won't pass until MVC 3, because there is a bug in UIHintAttribute which causes
            // only single instances of it to be returned from type descriptors. Dev10 Bugs #659308
            // If the application targets 4.0, they will get the correct behavior, because we've
            // implemented it, but the unit test targets 3.5 SP1 and therefore won't pass. As a work-
            // around, users who target 3.5 should only use a single [UIHint] attribute.

            Assert.AreEqual("MyMvcTemplate", provider.GetMetadataForProperty(typeof(UIHintModel), "MultipleUIHint").TemplateHint);
#endif
        }

        class DataTypeModel {
            public int NoAttribute { get; set; }

            [DataType(DataType.EmailAddress)]
            public int EmailAddressProperty { get; set; }

            [DataType("CustomDataType")]
            public int CustomDataTypeProperty { get; set; }
        }

        [TestMethod]
        public void DataTypeAttributeSetsDataTypeName() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act & Assert
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DataTypeModel), "NoAttribute").DataTypeName);
            Assert.AreEqual("EmailAddress", provider.GetMetadataForProperty(null, typeof(DataTypeModel), "EmailAddressProperty").DataTypeName);
            Assert.AreEqual("CustomDataType", provider.GetMetadataForProperty(null, typeof(DataTypeModel), "CustomDataTypeProperty").DataTypeName);
        }

        class IsReadOnlyModel {
            public int NoAttribute { get; set; }

            [ReadOnly(true)]
            public int ReadOnlyTrue { get; set; }

            [ReadOnly(false)]
            public int ReadOnlyFalse { get; set; }
        }

        [TestMethod]
        public void ReadOnlyAttributeSetsIsReadOnly() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act & Assert
            Assert.IsFalse(provider.GetMetadataForProperty(null, typeof(IsReadOnlyModel), "NoAttribute").IsReadOnly);
            Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(IsReadOnlyModel), "ReadOnlyTrue").IsReadOnly);
            Assert.IsFalse(provider.GetMetadataForProperty(null, typeof(IsReadOnlyModel), "ReadOnlyFalse").IsReadOnly);
        }

        class DisplayFormatModel {
            public int NoAttribute { get; set; }

            [DisplayFormat(NullDisplayText = "(null value)")]
            public int NullDisplayText { get; set; }

            [DisplayFormat(DataFormatString = "Data {0} format")]
            public int DisplayFormatString { get; set; }

            [DisplayFormat(DataFormatString = "Data {0} format", ApplyFormatInEditMode = true)]
            public int DisplayAndEditFormatString { get; set; }

            [DisplayFormat(ConvertEmptyStringToNull = true)]
            public int ConvertEmptyStringToNullTrue { get; set; }

            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public int ConvertEmptyStringToNullFalse { get; set; }

            [DataType(DataType.Currency)]
            public int DataTypeWithoutDisplayFormatOverride { get; set; }

            [DataType(DataType.Currency)]
            [DisplayFormat(DataFormatString = "format override")]
            public int DataTypeWithDisplayFormatOverride { get; set; }
        }

        [TestMethod]
        public void DisplayFormaAttributetSetsNullDisplayText() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act & Assert
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "NoAttribute").NullDisplayText);
            Assert.AreEqual("(null value)", provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "NullDisplayText").NullDisplayText);
        }

        [TestMethod]
        public void DisplayFormatAttributeSetsDisplayFormatString() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act & Assert
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "NoAttribute").DisplayFormatString);
            Assert.AreEqual("Data {0} format", provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "DisplayFormatString").DisplayFormatString);
            Assert.AreEqual("Data {0} format", provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "DisplayAndEditFormatString").DisplayFormatString);
        }

        [TestMethod]
        public void DisplayFormatAttributeSetEditFormatString() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act & Assert
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "NoAttribute").EditFormatString);
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "DisplayFormatString").EditFormatString);
            Assert.AreEqual("Data {0} format", provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "DisplayAndEditFormatString").EditFormatString);
        }

        [TestMethod]
        public void DisplayFormatAttributeSetsConvertEmptyStringToNull() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act & Assert
            Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "NoAttribute").ConvertEmptyStringToNull);
            Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "ConvertEmptyStringToNullTrue").ConvertEmptyStringToNull);
            Assert.IsFalse(provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "ConvertEmptyStringToNullFalse").ConvertEmptyStringToNull);
        }

        [TestMethod]
        public void DataTypeWithoutDisplayFormatOverrideUsesDataTypesDisplayFormat() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act
            string result = provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "DataTypeWithoutDisplayFormatOverride").DisplayFormatString;

            // Assert
            Assert.AreEqual("{0:C}", result);    // Currency's default format string
        }

        [TestMethod]
        public void DataTypeWithDisplayFormatOverrideUsesDisplayFormatOverride() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act
            string result = provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "DataTypeWithDisplayFormatOverride").DisplayFormatString;

            // Assert
            Assert.AreEqual("format override", result);
        }

        class ScaffoldColumnModel {
            public int NoAttribute { get; set; }

            [ScaffoldColumn(true)]
            public int ScaffoldColumnTrue { get; set; }

            [ScaffoldColumn(false)]
            public int ScaffoldColumnFalse { get; set; }
        }

        [TestMethod]
        public void ScaffoldColumnAttributeSetsShowForDisplay() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act & Assert
            Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(ScaffoldColumnModel), "NoAttribute").ShowForDisplay);
            Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(ScaffoldColumnModel), "ScaffoldColumnTrue").ShowForDisplay);
            Assert.IsFalse(provider.GetMetadataForProperty(null, typeof(ScaffoldColumnModel), "ScaffoldColumnFalse").ShowForDisplay);
        }

        [TestMethod]
        public void ScaffoldColumnAttributeSetsShowForEdit() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act & Assert
            Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(ScaffoldColumnModel), "NoAttribute").ShowForEdit);
            Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(ScaffoldColumnModel), "ScaffoldColumnTrue").ShowForEdit);
            Assert.IsFalse(provider.GetMetadataForProperty(null, typeof(ScaffoldColumnModel), "ScaffoldColumnFalse").ShowForEdit);
        }

        [DisplayColumn("NoPropertyWithThisName")]
        class UnknownDisplayColumnModel { }

        [TestMethod]
        public void SimpleDisplayNameWithUnknownDisplayColumnThrows() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => provider.GetMetadataForType(() => new UnknownDisplayColumnModel(), typeof(UnknownDisplayColumnModel)).SimpleDisplayText,
                "System.Web.Mvc.Test.DataAnnotationsModelMetadataProviderTest+UnknownDisplayColumnModel has a DisplayColumn attribute for NoPropertyWithThisName, but property NoPropertyWithThisName does not exist.");
        }

        [DisplayColumn("WriteOnlyProperty")]
        class WriteOnlyDisplayColumnModel {
            public int WriteOnlyProperty { set { } }
        }

        [DisplayColumn("PrivateReadPublicWriteProperty")]
        class PrivateReadPublicWriteDisplayColumnModel {
            public int PrivateReadPublicWriteProperty { private get; set; }
        }

        [TestMethod]
        public void SimpleDisplayTextForTypeWithWriteOnlyDisplayColumnThrows() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => provider.GetMetadataForType(() => new WriteOnlyDisplayColumnModel(), typeof(WriteOnlyDisplayColumnModel)).SimpleDisplayText,
                "System.Web.Mvc.Test.DataAnnotationsModelMetadataProviderTest+WriteOnlyDisplayColumnModel has a DisplayColumn attribute for WriteOnlyProperty, but property WriteOnlyProperty does not have a public getter.");

            ExceptionHelper.ExpectInvalidOperationException(
                () => provider.GetMetadataForType(() => new PrivateReadPublicWriteDisplayColumnModel(), typeof(PrivateReadPublicWriteDisplayColumnModel)).SimpleDisplayText,
                "System.Web.Mvc.Test.DataAnnotationsModelMetadataProviderTest+PrivateReadPublicWriteDisplayColumnModel has a DisplayColumn attribute for PrivateReadPublicWriteProperty, but property PrivateReadPublicWriteProperty does not have a public getter.");
        }

        [DisplayColumn("DisplayColumnProperty")]
        class SimpleDisplayTextAttributeModel {
            public int FirstProperty { get { return 42; } }

            [ScaffoldColumn(false)]
            public string DisplayColumnProperty { get; set; }
        }

        [TestMethod]
        public void SimpleDisplayTextForNonNullClassWithNonNullDisplayColumnValue() {
            // Arrange
            string expected = "Custom property display value";
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();
            SimpleDisplayTextAttributeModel model = new SimpleDisplayTextAttributeModel { DisplayColumnProperty = expected };
            ModelMetadata metadata = provider.GetMetadataForType(() => model, typeof(SimpleDisplayTextAttributeModel));

            // Act
            string result = metadata.SimpleDisplayText;

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void SimpleDisplayTextForNullClassRevertsToDefaultBehavior() {
            // Arrange
            string expected = "Null Display Text";
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();
            ModelMetadata metadata = provider.GetMetadataForType(null, typeof(SimpleDisplayTextAttributeModel));
            metadata.NullDisplayText = expected;

            // Act
            string result = metadata.SimpleDisplayText;

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void SimpleDisplayTextForNonNullClassWithNullDisplayColumnValueRevertsToDefaultBehavior() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();
            SimpleDisplayTextAttributeModel model = new SimpleDisplayTextAttributeModel();
            ModelMetadata metadata = provider.GetMetadataForType(() => model, typeof(SimpleDisplayTextAttributeModel));

            // Act
            string result = metadata.SimpleDisplayText;

            // Assert
            Assert.AreEqual("42", result);    // Falls back to the default logic of first property value
        }

        class DisplayNameAttributeModel {
            public int Without { get; set; }

            [DisplayName("Custom property name")]
            public int With { get; set; }
        }

        [TestMethod]
        public void DisplayNameAttributeSetsDisplayName() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act & Assert
            Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DisplayNameAttributeModel), "Without").DisplayName);
            Assert.AreEqual("Custom property name", provider.GetMetadataForProperty(null, typeof(DisplayNameAttributeModel), "With").DisplayName);
        }

        class IsRequiredModel {
            public int NonNullableWithout { get; set; }

            public string NullableWithout { get; set; }

            [Required]
            public string NullableWith { get; set; }
        }

        [TestMethod]
        public void IsRequiredTests() {
            // Arrange
            DataAnnotationsModelMetadataProvider provider = new DataAnnotationsModelMetadataProvider();

            // Act & Assert
            Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(IsRequiredModel), "NonNullableWithout").IsRequired);
            Assert.IsFalse(provider.GetMetadataForProperty(null, typeof(IsRequiredModel), "NullableWithout").IsRequired);
            Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(IsRequiredModel), "NullableWith").IsRequired);
        }
    }
}
