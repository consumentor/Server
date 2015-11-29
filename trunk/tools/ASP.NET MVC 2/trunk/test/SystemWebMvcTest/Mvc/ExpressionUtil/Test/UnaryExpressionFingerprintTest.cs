namespace System.Web.Mvc.ExpressionUtil.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UnaryExpressionFingerprintTest {

        [TestMethod]
        public void Comparison_DifferByMethod() {
            // fingerprint1 is OP_UNARYPLUS(TimeSpan.op_UnaryPlus, PARAM:TimeSpan):TimeSpan
            // fingerprint2 is OP_UNARYPLUS(TimeSpan.op_UnaryNegation, PARAM:TimeSpan):TimeSpan, so not equivalent
            // might be strange using op_UnaryNegation() for UNARYPLUS, but at least it makes the Methods different

            // Arrange
            MethodInfo unaryPlusMethod = typeof(TimeSpan).GetMethod("op_UnaryPlus", BindingFlags.Static | BindingFlags.Public);
            MethodInfo negateMethod = typeof(TimeSpan).GetMethod("op_UnaryNegation", BindingFlags.Static | BindingFlags.Public);

            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(TimeSpan), "model") };
            ExpressionFingerprint fingerprint1 = UnaryExpressionFingerprint.Create(Expression.UnaryPlus(context.ModelParameter, unaryPlusMethod), context);
            ExpressionFingerprint fingerprint2 = UnaryExpressionFingerprint.Create(Expression.UnaryPlus(context.ModelParameter, negateMethod), context);

            // Act
            bool areEqual = Object.Equals(fingerprint1, fingerprint2);

            // Assert
            Assert.IsFalse(areEqual, "Fingerprints should not have been equivalent - differ by UnaryExpressionFingerprint.Method.");
        }

        [TestMethod]
        public void Comparison_DifferByOperand() {
            // fingerprints are OP_NEGATE(CONST:int):int and OP_NEGATE(PARAM:int):int, so not equivalent

            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(int), "model") };
            ExpressionFingerprint fingerprint1 = UnaryExpressionFingerprint.Create(Expression.Negate(Expression.Constant(0)), context);
            ExpressionFingerprint fingerprint2 = UnaryExpressionFingerprint.Create(Expression.Negate(context.ModelParameter), context);

            // Act
            bool areEqual = Object.Equals(fingerprint1, fingerprint2);

            // Assert
            Assert.IsFalse(areEqual, "Fingerprints should not have been equivalent - differ by UnaryExpressionFingerprint.Operand.");
        }

        [TestMethod]
        public void Comparison_EquivalentExpressions() {
            // both fingerprints are OP_NEGATE(CONST:TimeSpan):TimeSpan, so are equivalent

            // Arrange
            ExpressionFingerprint fingerprint1 = UnaryExpressionFingerprint.Create(Expression.Negate(Expression.Constant(TimeSpan.MinValue)), new ParserContext());
            ExpressionFingerprint fingerprint2 = UnaryExpressionFingerprint.Create(Expression.Negate(Expression.Constant(TimeSpan.MaxValue)), new ParserContext());

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
            ParserContext context = new ParserContext();
            Expression operand = Expression.Constant(TimeSpan.Zero);
            UnaryExpression expression = Expression.Negate(operand);

            // Act
            UnaryExpressionFingerprint fingerprint = UnaryExpressionFingerprint.Create(expression, context);

            // Assert
            Assert.AreEqual(expression.Method, fingerprint.Method);
            Assert.AreEqual(expression.Operand.NodeType, fingerprint.Operand.NodeType);
        }

        [TestMethod]
        public void Create_UnknownOperand() {
            // Arrange
            ParserContext context = new ParserContext();
            Expression operand = ExpressionHelper.GetUnknownExpression();
            UnaryExpression expression = Expression.Convert(operand, typeof(object));

            // Act
            UnaryExpressionFingerprint fingerprint = UnaryExpressionFingerprint.Create(expression, context);

            // Assert
            Assert.IsNull(fingerprint, "Unknown operands cannot be parsed.");
        }

        [TestMethod]
        public void ToExpression() {
            // Arrange
            ParserContext context = new ParserContext();
            UnaryExpression expression = Expression.Negate(Expression.Constant(TimeSpan.Zero));
            UnaryExpressionFingerprint fingerprint = UnaryExpressionFingerprint.Create(expression, context);

            // Act
            UnaryExpression result = (UnaryExpression)fingerprint.ToExpression(context);

            // Assert
            Assert.AreEqual(ExpressionType.Negate, result.NodeType);
            Assert.AreEqual(ExpressionType.Convert, result.Operand.NodeType); // constants are converted to casts
        }

        [TestMethod]
        public void ToExpression_UnaryPlus() {
            // in .NET 3.5 SP1, Expression.MakeUnary() throws if NodeType is UnaryPlus,
            // so this is special-cased by UnaryExpressionFingerprint

            // Arrange
            ParserContext context = new ParserContext();
            UnaryExpression expression = Expression.UnaryPlus(Expression.Constant(TimeSpan.Zero));
            UnaryExpressionFingerprint fingerprint = UnaryExpressionFingerprint.Create(expression, context);

            // Act
            UnaryExpression result = (UnaryExpression)fingerprint.ToExpression(context);

            // Assert
            Assert.AreEqual(ExpressionType.UnaryPlus, result.NodeType);
            Assert.AreEqual(ExpressionType.Convert, result.Operand.NodeType); // constants are converted to casts
        }

    }
}
