namespace Microsoft.Web.Mvc.AspNet4.Test {
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StringLengthAttribute4AdapterTest {

        [TestMethod]
        public void ClientRulesWithStringLengthAttribute() {
            // Arrange
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => null, typeof(string), "Length");
            var context = new ControllerContext();
            var attribute = new StringLengthAttribute(10) { MinimumLength = 3 };
            var adapter = new StringLengthAttribute4Adapter(metadata, context, attribute);

            // Act
            var rules = adapter.GetClientValidationRules()
                               .OrderBy(r => r.ValidationType)
                               .ToArray();

            // Assert
            Assert.AreEqual(1, rules.Length);

            Assert.AreEqual("stringLength", rules[0].ValidationType);
            Assert.AreEqual(2, rules[0].ValidationParameters.Count);
            Assert.AreEqual(3, rules[0].ValidationParameters["minimumLength"]);
            Assert.AreEqual(10, rules[0].ValidationParameters["maximumLength"]);
            Assert.AreEqual(@"The field Length must be a string with a minimum length of 3 and a maximum length of 10.", rules[0].ErrorMessage);
        }

    }
}
