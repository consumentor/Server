namespace System.Web.Mvc.ExpressionUtil.Test {
    using System;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExpressionParserTest {

        [TestMethod]
        public void Parse_Equivalent() {
            // tests lots of parsing, e.g. casts, constants, array lookups, etc.

            // Arrange
            var person = new[] {
                new { FirstName = "John", LastName = "Doe" },
                new { FirstName = "Jane", LastName = "Doe" }
            };
            Expression<Func<int, string>> expr1 = model => ((true) ? person[-model].FirstName : person[0].LastName).ToUpper();
            Expression<Func<int, string>> expr2 = model => ((true) ? person[-model].FirstName : person[1].LastName).ToUpper();

            // Act
            ParserContext context1 = ExpressionParser.Parse(expr1);
            ParserContext context2 = ExpressionParser.Parse(expr2);

            // Assert
            Assert.IsNotNull(context1.Fingerprint, "Expression fingerprinting should have succeeded.");
            Assert.AreEqual(context1.Fingerprint, context2.Fingerprint, "Fingerprints should be equal.");
        }

        [TestMethod]
        public void Parse_NotEquivalent() {
            // tests lots of parsing, e.g. casts, constants, array lookups, etc.

            // Arrange
            var person = new[] {
                new { FirstName = "John", LastName = "Doe" },
                new { FirstName = "Jane", LastName = "Doe" }
            };

            // expressions differ in person[-model] and person[+model]
            Expression<Func<int, string>> expr1 = model => ((true) ? person[-model].FirstName : person[0].LastName).ToUpper();
            Expression<Func<int, string>> expr2 = model => ((true) ? person[+model].FirstName : person[1].LastName).ToUpper();

            // Act
            ParserContext context1 = ExpressionParser.Parse(expr1);
            ParserContext context2 = ExpressionParser.Parse(expr2);

            // Assert
            Assert.IsNotNull(context1.Fingerprint, "Expression1 fingerprinting should have succeeded.");
            Assert.IsNotNull(context2.Fingerprint, "Expression2 fingerprinting should have succeeded.");
            Assert.AreNotEqual(context1.Fingerprint, context2.Fingerprint, "Fingerprints should not be equal.");
        }

    }
}
