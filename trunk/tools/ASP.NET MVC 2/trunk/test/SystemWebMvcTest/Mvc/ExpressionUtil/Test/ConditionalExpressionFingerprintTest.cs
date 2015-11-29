namespace System.Web.Mvc.ExpressionUtil.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConditionalExpressionFingerprintTest {

        [TestMethod]
        public void Comparison_DifferByIfFalse() {
            // fingerprint1 is IIF(CONST:bool, CONST:TimeSpan, CONST:TimeSpan):TimeSpan
            // fingerprint2 is IIF(CONST:bool, CONST:TimeSpan, OP_NEGATE(CONST:TimeSpan):TimeSpan):TimeSpan, so not equivalent

            // Arrange
            ExpressionFingerprint fingerprint1 = ConditionalExpressionFingerprint.Create(Expression.Condition(Expression.Constant(true), Expression.Constant(TimeSpan.MinValue), Expression.Constant(TimeSpan.MaxValue)), new ParserContext());
            ExpressionFingerprint fingerprint2 = ConditionalExpressionFingerprint.Create(Expression.Condition(Expression.Constant(true), Expression.Constant(TimeSpan.MaxValue), Expression.Negate(Expression.Constant(TimeSpan.Zero))), new ParserContext());

            // Act
            bool areEqual = Object.Equals(fingerprint1, fingerprint2);

            // Assert
            Assert.IsFalse(areEqual, "Fingerprints should not have been equivalent - differ by ConditionalExpressionFingerprint.IfTrue.");
        }

        [TestMethod]
        public void Comparison_DifferByIfTrue() {
            // fingerprint1 is IIF(CONST:bool, CONST:TimeSpan, CONST:TimeSpan):TimeSpan
            // fingerprint2 is IIF(CONST:bool, OP_UNARYPLUS(CONST:TimeSpan):TimeSpan, CONST:TimeSpan):TimeSpan, so not equivalent

            // Arrange
            ExpressionFingerprint fingerprint1 = ConditionalExpressionFingerprint.Create(Expression.Condition(Expression.Constant(true), Expression.Constant(TimeSpan.MinValue), Expression.Constant(TimeSpan.MaxValue)), new ParserContext());
            ExpressionFingerprint fingerprint2 = ConditionalExpressionFingerprint.Create(Expression.Condition(Expression.Constant(true), Expression.UnaryPlus(Expression.Constant(TimeSpan.MaxValue)), Expression.Constant(TimeSpan.Zero)), new ParserContext());

            // Act
            bool areEqual = Object.Equals(fingerprint1, fingerprint2);

            // Assert
            Assert.IsFalse(areEqual, "Fingerprints should not have been equivalent - differ by ConditionalExpressionFingerprint.IfTrue.");
        }

        [TestMethod]
        public void Comparison_DifferByTest() {
            // fingerprint1 is IIF(CONST:bool, CONST:TimeSpan, CONST:TimeSpan):TimeSpan
            // fingerprint2 is IIF(OP_NOT(CONST:bool):bool, CONST:TimeSpan, CONST:TimeSpan):TimeSpan, so not equivalent

            // Arrange
            ExpressionFingerprint fingerprint1 = ConditionalExpressionFingerprint.Create(Expression.Condition(Expression.Constant(true), Expression.Constant(TimeSpan.MinValue), Expression.Constant(TimeSpan.MaxValue)), new ParserContext());
            ExpressionFingerprint fingerprint2 = ConditionalExpressionFingerprint.Create(Expression.Condition(Expression.Not(Expression.Constant(true)), Expression.Constant(TimeSpan.MaxValue), Expression.Constant(TimeSpan.Zero)), new ParserContext());

            // Act
            bool areEqual = Object.Equals(fingerprint1, fingerprint2);

            // Assert
            Assert.IsFalse(areEqual, "Fingerprints should not have been equivalent - differ by ConditionalExpressionFingerprint.Test.");
        }

        [TestMethod]
        public void Comparison_EquivalentExpressions() {
            // both fingerprints are IIF(CONST:bool, CONST:TimeSpan, CONST:TimeSpan):TimeSpan, so are equivalent

            // Arrange
            ExpressionFingerprint fingerprint1 = ConditionalExpressionFingerprint.Create(Expression.Condition(Expression.Constant(true), Expression.Constant(TimeSpan.MinValue), Expression.Constant(TimeSpan.MaxValue)), new ParserContext());
            ExpressionFingerprint fingerprint2 = ConditionalExpressionFingerprint.Create(Expression.Condition(Expression.Constant(true), Expression.Constant(TimeSpan.MaxValue), Expression.Constant(TimeSpan.Zero)), new ParserContext());

            // Act
            HashSet<ExpressionFingerprint> set = new HashSet<ExpressionFingerprint>() {
                fingerprint1,
                fingerprint2
            };

            // Assert
            Assert.AreEqual(1, set.Count, "Fingerprints should have been equivalent.");
        }

        [TestMethod]
        public void Create() {
            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(bool), "model") };
            Expression testExpr = context.ModelParameter;
            Expression ifTrueExpr = Expression.UnaryPlus(Expression.Constant(TimeSpan.Zero));
            Expression ifFalseExpr = Expression.Negate(Expression.Constant(TimeSpan.Zero));
            ConditionalExpression expression = Expression.Condition(testExpr, ifTrueExpr, ifFalseExpr);

            // Act
            ConditionalExpressionFingerprint fingerprint = ConditionalExpressionFingerprint.Create(expression, context);

            // Assert
            Assert.AreEqual(ExpressionType.Parameter, fingerprint.Test.NodeType);
            Assert.AreEqual(ExpressionType.UnaryPlus, fingerprint.IfTrue.NodeType);
            Assert.AreEqual(ExpressionType.Negate, fingerprint.IfFalse.NodeType);
        }

        [TestMethod]
        public void Create_UnknownIfFalse() {
            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(bool), "model") };
            Expression testExpr = context.ModelParameter;
            Expression ifTrueExpr = Expression.UnaryPlus(Expression.Constant(TimeSpan.Zero));
            Expression ifFalseExpr = ExpressionHelper.GetUnknownExpression(typeof(TimeSpan));
            ConditionalExpression expression = Expression.Condition(testExpr, ifTrueExpr, ifFalseExpr);

            // Act
            ConditionalExpressionFingerprint fingerprint = ConditionalExpressionFingerprint.Create(expression, context);

            // Assert
            Assert.IsNull(fingerprint, "Unknown IfFalse expression cannot be parsed.");
        }

        [TestMethod]
        public void Create_UnknownIfTrue() {
            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(bool), "model") };
            Expression testExpr = context.ModelParameter;
            Expression ifTrueExpr = ExpressionHelper.GetUnknownExpression(typeof(TimeSpan));
            Expression ifFalseExpr = Expression.Negate(Expression.Constant(TimeSpan.Zero));
            ConditionalExpression expression = Expression.Condition(testExpr, ifTrueExpr, ifFalseExpr);

            // Act
            ConditionalExpressionFingerprint fingerprint = ConditionalExpressionFingerprint.Create(expression, context);

            // Assert
            Assert.IsNull(fingerprint, "Unknown IfTrue expression cannot be parsed.");
        }

        [TestMethod]
        public void Create_UnknownTest() {
            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(bool), "model") };
            Expression testExpr = ExpressionHelper.GetUnknownExpression(typeof(bool));
            Expression ifTrueExpr = Expression.UnaryPlus(Expression.Constant(TimeSpan.Zero));
            Expression ifFalseExpr = Expression.Negate(Expression.Constant(TimeSpan.Zero));
            ConditionalExpression expression = Expression.Condition(testExpr, ifTrueExpr, ifFalseExpr);

            // Act
            ConditionalExpressionFingerprint fingerprint = ConditionalExpressionFingerprint.Create(expression, context);

            // Assert
            Assert.IsNull(fingerprint, "Unknown Test expression cannot be parsed.");
        }

        [TestMethod]
        public void ToExpression() {
            // Arrange
            ParserContext context = new ParserContext();
            Expression testExpr = Expression.Not(Expression.Constant(true));
            Expression ifTrueExpr = Expression.UnaryPlus(Expression.Constant(TimeSpan.Zero));
            Expression ifFalseExpr = Expression.Negate(Expression.Constant(TimeSpan.Zero));
            ConditionalExpression expression = Expression.Condition(testExpr, ifTrueExpr, ifFalseExpr);
            ConditionalExpressionFingerprint fingerprint = ConditionalExpressionFingerprint.Create(expression, context);

            // Act
            ConditionalExpression result = (ConditionalExpression)fingerprint.ToExpression(context);

            // Assert
            Assert.AreEqual(ExpressionType.Not, result.Test.NodeType);
            Assert.AreEqual(ExpressionType.UnaryPlus, result.IfTrue.NodeType);
            Assert.AreEqual(ExpressionType.Negate, result.IfFalse.NodeType);
        }

    }
}
