namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelClientValidationRuleTest {

        [TestMethod]
        public void ValidationParametersProperty() {
            // Arrange
            ModelClientValidationRule rule = new ModelClientValidationRule();

            // Act
            IDictionary<string, object> parameters = rule.ValidationParameters;

            // Assert
            Assert.IsNotNull(parameters);
            Assert.AreEqual(0, parameters.Count);
        }

        [TestMethod]
        public void ValidationTypeProperty() {
            // Arrange
            ModelClientValidationRule rule = new ModelClientValidationRule();

            // Act & assert
            MemberHelper.TestStringProperty(rule, "ValidationType", "", false /* testDefaultValue */);
        }

    }
}
