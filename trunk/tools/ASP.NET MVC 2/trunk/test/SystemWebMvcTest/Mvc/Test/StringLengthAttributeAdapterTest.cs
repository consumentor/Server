namespace System.Web.Mvc.Test {
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StringLengthAttributeAdapterTest {

        [TestMethod]
        public void ClientRulesWithStringLengthAttribute() {
            // Arrange
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => null, typeof(string), "Length");
            var context = new ControllerContext();
            var attribute = new StringLengthAttribute(10);
            var adapter = new StringLengthAttributeAdapter(metadata, context, attribute);

            // Act
            var rules = adapter.GetClientValidationRules()
                               .OrderBy(r => r.ValidationType)
                               .ToArray();

            // Assert
            Assert.AreEqual(1, rules.Length);

            Assert.AreEqual("stringLength", rules[0].ValidationType);
            Assert.AreEqual(2, rules[0].ValidationParameters.Count);
            Assert.AreEqual(0, rules[0].ValidationParameters["minimumLength"]);
            Assert.AreEqual(10, rules[0].ValidationParameters["maximumLength"]);
            Assert.AreEqual(@"The field Length must be a string with a maximum length of 10.", rules[0].ErrorMessage);
        }

    }
}
