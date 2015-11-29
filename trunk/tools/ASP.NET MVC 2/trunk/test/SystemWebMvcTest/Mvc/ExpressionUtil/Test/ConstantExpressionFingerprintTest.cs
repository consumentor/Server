namespace System.Web.Mvc.ExpressionUtil.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConstantExpressionFingerprintTest {

        [TestMethod]
        public void Comparison_DifferByHoistedLocalsIndex() {
            // fingerprints are CONST[0]:int and CONST[1]:int, so not equivalent

            // Arrange
            ParserContext context = new ParserContext();
            ExpressionFingerprint fingerprint1 = ConstantExpressionFingerprint.Create(Expression.Constant(0), context);
            ExpressionFingerprint fingerprint2 = ConstantExpressionFingerprint.Create(Expression.Constant(0), context);

            // Act
            bool areEqual = Object.Equals(fingerprint1, fingerprint2);

            // Assert
            Assert.IsFalse(areEqual, "Fingerprints should not have been equivalent - differ by ConstantExpressionFingerprint.HoistedLocalsIndex.");
        }

        [TestMethod]
        public void Comparison_EquivalentExpressions() {
            // both fingerprints are CONST[0]:int, so are equivalent

            // Arrange
            ExpressionFingerprint fingerprint1 = ConstantExpressionFingerprint.Create(Expression.Constant(1), new ParserContext());
            ExpressionFingerprint fingerprint2 = ConstantExpressionFingerprint.Create(Expression.Constant(2), new ParserContext());

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
            ConstantExpression expression = Expression.Constant(42);
            ParserContext context = new ParserContext();
            context.HoistedValues.Add(null);
            context.HoistedValues.Add(null);

            // Act
            ConstantExpressionFingerprint fingerprint = ConstantExpressionFingerprint.Create(expression, context);

            // Assert
            Assert.AreEqual(2, fingerprint.HoistedLocalsIndex, "Index should point to the end of the list.");
            Assert.AreEqual(3, context.HoistedValues.Count, "List size should have been increased by one.");
            Assert.AreEqual(42, context.HoistedValues[2], "Value was not added to end of list.");
        }

        [TestMethod]
        public void ToExpression() {
            // Arrange
            ConstantExpression expression = Expression.Constant(42);
            ParserContext context = new ParserContext();
            context.HoistedValues.Add(null);
            context.HoistedValues.Add(null);

            ConstantExpressionFingerprint fingerprint = ConstantExpressionFingerprint.Create(expression, context);

            // Act
            Expression result = fingerprint.ToExpression(context);

            // Assert
            Assert.AreEqual(ExpressionType.Convert, result.NodeType, "Returned expression should have been a cast.");
            UnaryExpression castExpr = (UnaryExpression)result;
            Assert.AreEqual(typeof(int), castExpr.Type);

            Assert.AreEqual(ExpressionType.ArrayIndex, castExpr.Operand.NodeType, "Inner expression should have been an array lookup.");
            BinaryExpression arrayLookupExpr = (BinaryExpression)castExpr.Operand;
            Assert.AreEqual(ParserContext.HoistedValuesParameter, arrayLookupExpr.Left);

            Assert.AreEqual(ExpressionType.Constant, arrayLookupExpr.Right.NodeType, "Index of array lookup should be a constant expression.");
            ConstantExpression indexExpr = (ConstantExpression)arrayLookupExpr.Right;
            Assert.AreEqual(2, indexExpr.Value, "Wrong index output.");
        }

    }
}
