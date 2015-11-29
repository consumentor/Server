namespace System.Web.Mvc.Test {
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelValidationResultTest {

        [TestMethod]
        public void MemberNameProperty() {
            // Arrange
            ModelValidationResult result = new ModelValidationResult();

            // Act & assert
            MemberHelper.TestStringProperty(result, "MemberName", String.Empty, false);
        }

        [TestMethod]
        public void MessageProperty() {
            // Arrange
            ModelValidationResult result = new ModelValidationResult();

            // Act & assert
            MemberHelper.TestStringProperty(result, "Message", String.Empty, false);
        }

    }
}
