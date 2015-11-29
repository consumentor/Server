namespace Microsoft.Web.Mvc.AspNet4 {
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RangeAttribute4AdapterTest {

        [TestMethod]
        public void ClientRulesWithRangeAttribute() {
            // Arrange
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => null, typeof(string), "Length");
            var context = new ControllerContext();
            var attribute = new RangeAttribute(typeof(decimal), "0", "100");
            var adapter = new RangeAttribute4Adapter(metadata, context, attribute);

            // Act
            var rules = adapter.GetClientValidationRules()
                               .OrderBy(r => r.ValidationType)
                               .ToArray();

            // Assert
            Assert.AreEqual(1, rules.Length);

            Assert.AreEqual("range", rules[0].ValidationType);
            Assert.AreEqual(2, rules[0].ValidationParameters.Count);
            Assert.AreEqual(0m, rules[0].ValidationParameters["minimum"]);
            Assert.AreEqual(100m, rules[0].ValidationParameters["maximum"]);
            Assert.AreEqual(@"The field Length must be between 0 and 100.", rules[0].ErrorMessage);
        }

    }
}
