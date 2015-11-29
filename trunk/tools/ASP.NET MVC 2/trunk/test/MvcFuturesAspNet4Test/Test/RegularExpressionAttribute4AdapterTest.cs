namespace Microsoft.Web.Mvc.AspNet4.Test {
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RegularExpressionAttribute4AdapterTest {

        [TestMethod]
        public void ClientRulesWithRegexAttribute() {
            // Arrange
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => null, typeof(string), "Length");
            var context = new ControllerContext();
            var attribute = new RegularExpressionAttribute("the_pattern");
            var adapter = new RegularExpressionAttribute4Adapter(metadata, context, attribute);

            // Act
            var rules = adapter.GetClientValidationRules()
                               .OrderBy(r => r.ValidationType)
                               .ToArray();

            // Assert
            Assert.AreEqual(1, rules.Length);

            Assert.AreEqual("regularExpression", rules[0].ValidationType);
            Assert.AreEqual(1, rules[0].ValidationParameters.Count);
            Assert.AreEqual("the_pattern", rules[0].ValidationParameters["pattern"]);
            Assert.AreEqual(@"The field Length must match the regular expression 'the_pattern'.", rules[0].ErrorMessage);
        }

    }
}
