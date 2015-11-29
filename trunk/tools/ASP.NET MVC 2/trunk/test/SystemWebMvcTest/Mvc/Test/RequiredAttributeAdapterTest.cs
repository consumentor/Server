namespace System.Web.Mvc.Test {
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RequiredAttributeAdapterTest {

        [TestMethod]
        public void ClientRulesWithRequiredAttribute() {
            // Arrange
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => null, typeof(string), "Length");
            var context = new ControllerContext();
            var attribute = new RequiredAttribute();
            var adapter = new RequiredAttributeAdapter(metadata, context, attribute);

            // Act
            var rules = adapter.GetClientValidationRules()
                               .OrderBy(r => r.ValidationType)
                               .ToArray();

            // Assert
            Assert.AreEqual(1, rules.Length);
            Assert.AreEqual("required", rules[0].ValidationType);
            Assert.AreEqual(0, rules[0].ValidationParameters.Count);
            Assert.AreEqual(@"The Length field is required.", rules[0].ErrorMessage);
        }

    }
}
