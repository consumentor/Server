namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class FormContextTest {

        [TestMethod]
        public void FieldValidatorsProperty() {
            // Arrange
            FormContext context = new FormContext();

            // Act
            IDictionary<String, FieldValidationMetadata> fieldValidators = context.FieldValidators;

            // Assert
            Assert.IsNotNull(fieldValidators);
            Assert.AreEqual(0, fieldValidators.Count);
        }

        [TestMethod]
        public void ReplaceValidationSummaryProperty() {
            // Arrange
            FormContext context = new FormContext();

            // Act & Assert
            MemberHelper.TestBooleanProperty(context, "ReplaceValidationSummary", false, false);
        }

        [TestMethod]
        public void GetJsonValidationMetadata_NoValidationSummary() {
            // Arrange
            FormContext context = new FormContext() { FormId = "theFormId" };

            ModelClientValidationRule rule = new ModelClientValidationRule() { ValidationType = "ValidationType1", ErrorMessage = "Error Message" };
            rule.ValidationParameters["theParam"] = new { FirstName = "John", LastName = "Doe", Age = 32 };
            FieldValidationMetadata metadata = new FieldValidationMetadata() { FieldName = "theFieldName", ValidationMessageId = "theFieldName_ValidationMessage" };
            metadata.ValidationRules.Add(rule);
            context.FieldValidators["theFieldName"] = metadata;

            // Act
            string jsonMetadata = context.GetJsonValidationMetadata();

            // Assert
            string expected = @"{""Fields"":[{""FieldName"":""theFieldName"",""ReplaceValidationMessageContents"":false,""ValidationMessageId"":""theFieldName_ValidationMessage"",""ValidationRules"":[{""ErrorMessage"":""Error Message"",""ValidationParameters"":{""theParam"":{""FirstName"":""John"",""LastName"":""Doe"",""Age"":32}},""ValidationType"":""ValidationType1""}]}],""FormId"":""theFormId"",""ReplaceValidationSummary"":false}";
            Assert.AreEqual(expected, jsonMetadata);
        }

        [TestMethod]
        public void GetJsonValidationMetadata_ValidationSummary() {
            // Arrange
            FormContext context = new FormContext() { FormId = "theFormId", ValidationSummaryId = "validationSummary" };

            ModelClientValidationRule rule = new ModelClientValidationRule() { ValidationType = "ValidationType1", ErrorMessage = "Error Message" };
            rule.ValidationParameters["theParam"] = new { FirstName = "John", LastName = "Doe", Age = 32 };
            FieldValidationMetadata metadata = new FieldValidationMetadata() { FieldName = "theFieldName", ValidationMessageId = "theFieldName_ValidationMessage" };
            metadata.ValidationRules.Add(rule);
            context.FieldValidators["theFieldName"] = metadata;

            // Act
            string jsonMetadata = context.GetJsonValidationMetadata();

            // Assert
            string expected = @"{""Fields"":[{""FieldName"":""theFieldName"",""ReplaceValidationMessageContents"":false,""ValidationMessageId"":""theFieldName_ValidationMessage"",""ValidationRules"":[{""ErrorMessage"":""Error Message"",""ValidationParameters"":{""theParam"":{""FirstName"":""John"",""LastName"":""Doe"",""Age"":32}},""ValidationType"":""ValidationType1""}]}],""FormId"":""theFormId"",""ReplaceValidationSummary"":false,""ValidationSummaryId"":""validationSummary""}";
            Assert.AreEqual(expected, jsonMetadata);
        }

        [TestMethod]
        public void GetValidationMetadataForField_Create_CreatesNewMetadataIfNotFound() {
            // Arrange
            FormContext context = new FormContext();

            // Act
            FieldValidationMetadata result = context.GetValidationMetadataForField("fieldName", true /* createIfNotFound */);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("fieldName", result.FieldName);

            Assert.AreEqual(1, context.FieldValidators.Count, "New metadata should have been added to FieldValidators.");
            Assert.AreEqual(result, context.FieldValidators["fieldName"]);
        }

        [TestMethod]
        public void GetValidationMetadataForField_NoCreate_ReturnsMetadataIfFound() {
            // Arrange
            FormContext context = new FormContext();
            FieldValidationMetadata metadata = new FieldValidationMetadata();
            context.FieldValidators["fieldName"] = metadata;

            // Act
            FieldValidationMetadata result = context.GetValidationMetadataForField("fieldName");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(metadata, result);
        }

        [TestMethod]
        public void GetValidationMetadataForField_NoCreate_ReturnsNullIfNotFound() {
            // Arrange
            FormContext context = new FormContext();

            // Act
            FieldValidationMetadata result = context.GetValidationMetadataForField("fieldName");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetValidationMetadataForFieldThrowsIfFieldNameIsEmpty() {
            // Arrange
            FormContext context = new FormContext();

            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    context.GetValidationMetadataForField(String.Empty);
                }, "fieldName");
        }

        [TestMethod]
        public void GetValidationMetadataForFieldThrowsIfFieldNameIsNull() {
            // Arrange
            FormContext context = new FormContext();

            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    context.GetValidationMetadataForField(null);
                }, "fieldName");
        }
    }
}
