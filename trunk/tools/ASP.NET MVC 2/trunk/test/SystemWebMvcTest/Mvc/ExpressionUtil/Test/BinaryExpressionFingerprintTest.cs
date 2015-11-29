namespace System.Web.Mvc.ExpressionUtil.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BinaryExpressionFingerprintTest {

        [TestMethod]
        public void Comparison_DifferByLeft() {
            // fingerprint1 is OP_ADD(PARAM:int, CONST:int):int
            // fingerprint2 is OP_ADD(CONST:int, CONST:int):int, so not equivalent

            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(int), "model") };
            ExpressionFingerprint fingerprint1 = BinaryExpressionFingerprint.Create(Expression.Add(context.ModelParameter, Expression.Constant(0)), context);
            ExpressionFingerprint fingerprint2 = BinaryExpressionFingerprint.Create(Expression.Add(Expression.Constant(0), Expression.Constant(0)), context);

            // Act
            bool areEqual = Object.Equals(fingerprint1, fingerprint2);

            // Assert
            Assert.IsFalse(areEqual, "Fingerprints should not have been equivalent - differ by BinaryExpressionFingerprint.Left.");
        }

        [TestMethod]
        public void Comparison_DifferByMethod() {
            // fingerprint1 is OP_ADD(TimeSpan.op_Addition, CONST:TimeSpan, CONST:TimeSpan):TimeSpan
            // fingerprint2 is OP_ADD(TimeSpan.op_Subtraction, CONST:TimeSpan, CONST:TimeSpan):TimeSpan, so not equivalent
            // might be strange using op_Subtraction() for ADD, but at least it makes the Methods different

            // Arrange
            MethodInfo addMethod = typeof(TimeSpan).GetMethod("op_Addition", BindingFlags.Static | BindingFlags.Public);
            MethodInfo subtractMethod = typeof(TimeSpan).GetMethod("op_Subtraction", BindingFlags.Static | BindingFlags.Public);

            ExpressionFingerprint fingerprint1 = BinaryExpressionFingerprint.Create(Expression.Add(Expression.Constant(TimeSpan.Zero), Expression.Constant(TimeSpan.Zero), addMethod), new ParserContext());
            ExpressionFingerprint fingerprint2 = BinaryExpressionFingerprint.Create(Expression.Add(Expression.Constant(TimeSpan.Zero), Expression.Constant(TimeSpan.Zero), subtractMethod), new ParserContext());

            // Act
            bool areEqual = Object.Equals(fingerprint1, fingerprint2);

            // Assert
            Assert.IsFalse(areEqual, "Fingerprints should not have been equivalent - differ by BinaryExpressionFingerprint.Method.");
        }

        [TestMethod]
        public void Comparison_DifferByRight() {
            // fingerprint1 is OP_ADD(CONST:int, PARAM:int):int
            // fingerprint2 is OP_ADD(CONST:int, CONST:int):int, so not equivalent

            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(int), "model") };
            ExpressionFingerprint fingerprint1 = BinaryExpressionFingerprint.Create(Expression.Add(Expression.Constant(0), context.ModelParameter), context);
            ExpressionFingerprint fingerprint2 = BinaryExpressionFingerprint.Create(Expression.Add(Expression.Constant(0), Expression.Constant(0)), context);

            // Act
            bool areEqual = Object.Equals(fingerprint1, fingerprint2);

            // Assert
            Assert.IsFalse(areEqual, "Fingerprints should not have been equivalent - differ by BinaryExpressionFingerprint.Right.");
        }

        [TestMethod]
        public void Comparison_EquivalentExpressions() {
            // both fingerprints are ARRAY_INDEX(CONST:object[], CONST:int):object, so are equivalent

            // Arrange
            ExpressionFingerprint fingerprint1 = BinaryExpressionFingerprint.Create(Expression.ArrayIndex(Expression.Constant(new object[0]), Expression.Constant(0)), new ParserContext());
            ExpressionFingerprint fingerprint2 = BinaryExpressionFingerprint.Create(Expression.ArrayIndex(Expression.Constant(new object[0]), Expression.Constant(0)), new ParserContext());

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
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(object[]), "model") };
            BinaryExpression expression = Expression.ArrayIndex(context.ModelParameter, Expression.Constant(5));

            // Act
            BinaryExpressionFingerprint fingerprint = BinaryExpressionFingerprint.Create(expression, context);

            // Assert
            Assert.AreEqual(ExpressionType.ArrayIndex, fingerprint.NodeType);
            Assert.AreEqual(ExpressionType.Parameter, fingerprint.Left.NodeType);
            Assert.AreEqual(ExpressionType.Constant, fingerprint.Right.NodeType);
        }

        [TestMethod]
        public void Create_ConversionLambdasAreRejected() {
            // Arrange
            Expression<Func<int?, int?>> identityFunc = i => i;
            BinaryExpression expression = Expression.Coalesce(Expression.Constant(5, typeof(int?)), Expression.Constant(6, typeof(int?)), identityFunc);

            // Act
            BinaryExpressionFingerprint fingerprint = BinaryExpressionFingerprint.Create(expression, new ParserContext());

            // Assert
            Assert.IsNull(fingerprint, "Any BinaryExpression with a non-null Conversion property should be rejected.");
        }

        [TestMethod]
        public void Create_UnknownLeft() {
            // Arrange
            BinaryExpression expression = Expression.Coalesce(ExpressionHelper.GetUnknownExpression(typeof(int?)), Expression.Constant(5, typeof(int?)));

            // Act
            BinaryExpressionFingerprint fingerprint = BinaryExpressionFingerprint.Create(expression, new ParserContext());

            // Assert
            Assert.IsNull(fingerprint, "Unknown left operands cannot be parsed.");
        }

        [TestMethod]
        public void Create_UnknownRight() {
            // Arrange
            BinaryExpression expression = Expression.Coalesce(Expression.Constant(5, typeof(int?)),ExpressionHelper.GetUnknownExpression(typeof(int?)));

            // Act
            BinaryExpressionFingerprint fingerprint = BinaryExpressionFingerprint.Create(expression, new ParserContext());

            // Assert
            Assert.IsNull(fingerprint, "Unknown right operands cannot be parsed.");
        }

        [TestMethod]
        public void ToExpression() {
            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(object[]), "model") };
            BinaryExpression expression = Expression.ArrayIndex(context.ModelParameter, Expression.Constant(5));
            BinaryExpressionFingerprint fingerprint = BinaryExpressionFingerprint.Create(expression, context);

            // Act
            BinaryExpression result = (BinaryExpression)fingerprint.ToExpression(context);

            // Assert
            Assert.AreEqual(ExpressionType.ArrayIndex, result.NodeType);
            Assert.AreEqual(ExpressionType.Parameter, result.Left.NodeType);
            Assert.AreEqual(ExpressionType.Convert, result.Right.NodeType);
        }

    }
}
