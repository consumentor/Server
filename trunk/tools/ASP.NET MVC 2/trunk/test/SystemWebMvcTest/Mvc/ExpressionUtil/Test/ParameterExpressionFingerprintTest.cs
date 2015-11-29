namespace System.Web.Mvc.ExpressionUtil.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ParameterExpressionFingerprintTest {

        [TestMethod]
        public void Create_DegenerateParameter() {
            // Arrange
            ParameterExpression expression = Expression.Parameter(typeof(int), null);
            ParserContext context = new ParserContext() {
                ModelParameter = Expression.Parameter(typeof(string), null) // different ParameterExpression
            };

            // Act
            ParameterExpressionFingerprint fingerprint = ParameterExpressionFingerprint.Create(expression, context);

            // Assert
            Assert.IsNull(fingerprint, "Degenerate ParameterExpression cannot be parsed.");
        }

        [TestMethod]
        public void Create_ModelParameter() {
            // Arrange
            ParameterExpression expression = Expression.Parameter(typeof(int), null);
            ParserContext context = new ParserContext() {
                ModelParameter = expression
            };

            // Act
            ParameterExpressionFingerprint fingerprint = ParameterExpressionFingerprint.Create(expression, context);

            // Assert
            Assert.IsNotNull(fingerprint);
        }

        [TestMethod]
        public void ToExpression() {
            // Arrange
            ParameterExpression expression = Expression.Parameter(typeof(int), null);
            ParserContext context = new ParserContext() {
                ModelParameter = expression
            };

            ParameterExpressionFingerprint fingerprint = ParameterExpressionFingerprint.Create(expression, context);

            // Act
            Expression result = fingerprint.ToExpression(context);

            // Assert
            Assert.AreEqual(expression, result, "Original model parameter should have been returned.");
        }

    }
}
