namespace System.Web.Mvc.Test {
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RegularExpressionAttributeAdapterTest {

        [TestMethod]
        public void ClientRulesWithRegexAttribute() {
            // Arrange
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => null, typeof(string), "Length");
            var context = new ControllerContext();
            var attribute = new RegularExpressionAttribute("the_pattern");
            var adapter = new RegularExpressionAttributeAdapter(metadata, context, attribute);

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
