namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FieldValidationMetadataTest {

        [TestMethod]
        public void FieldNameProperty() {
            // Arrange
            FieldValidationMetadata metadata = new FieldValidationMetadata();

            // Act & assert
            MemberHelper.TestStringProperty(metadata, "FieldName", "", false /* testDefaultValue */);
        }

        [TestMethod]
        public void ValidationRulesProperty() {
            // Arrange
            FieldValidationMetadata metadata = new FieldValidationMetadata();

            // Act
            ICollection<ModelClientValidationRule> validationRules = metadata.ValidationRules;

            // Assert
            Assert.IsNotNull(validationRules);
            Assert.AreEqual(0, validationRules.Count);
        }

    }
}
