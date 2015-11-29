namespace System.Web.Mvc.ExpressionUtil.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExpressionFingerprintTest {

        [TestMethod]
        public void Comparison_DifferByGetType() {
            // fingerprints are FOO:int and BAR:int, so not equivalent

            // Arrange
            ExpressionFingerprint fingerprint1 = new FooExpressionFingerprint(Expression.Constant(0));
            ExpressionFingerprint fingerprint2 = new BarExpressionFingerprint(Expression.Constant(0));

            // Act
            HashSet<ExpressionFingerprint> set = new HashSet<ExpressionFingerprint>() {
                fingerprint1,
                fingerprint2
            };

            // Assert
            Assert.AreEqual(2, set.Count, "Fingerprints should not have been equivalent - differ by ExpressionFingerprint.GetType().");
        }

        [TestMethod]
        public void Comparison_DifferByNodeType() {
            // fingerprints are FOO:int and PARAM:int, so not equivalent

            // Arrange
            ExpressionFingerprint fingerprint1 = new FooExpressionFingerprint(Expression.Constant(0));
            ExpressionFingerprint fingerprint2 = new FooExpressionFingerprint(Expression.Parameter(typeof(int), null));

            // Act
            HashSet<ExpressionFingerprint> set = new HashSet<ExpressionFingerprint>() {
                fingerprint1,
                fingerprint2
            };

            // Assert
            Assert.AreEqual(2, set.Count, "Fingerprints should not have been equivalent - differ by ExpressionFingerprint.NodeType.");
        }

        [TestMethod]
        public void Comparison_DifferByType() {
            // fingerprints are FOO:int and FOO:long, so not equivalent

            // Arrange
            ExpressionFingerprint fingerprint1 = new FooExpressionFingerprint(Expression.Constant(0));
            ExpressionFingerprint fingerprint2 = new FooExpressionFingerprint(Expression.Constant(0L));

            // Act
            HashSet<ExpressionFingerprint> set = new HashSet<ExpressionFingerprint>(){
                fingerprint1,
                fingerprint2
            };

            // Assert
            Assert.AreEqual(2, set.Count, "Fingerprints should not have been equivalent - differ by ExpressionFingerprint.Type.");
        }

        [TestMethod]
        public void Comparison_EquivalentExpressions() {
            // both fingerprints are FOO:int, so are equivalent

            // Arrange
            ExpressionFingerprint fingerprint1 = new FooExpressionFingerprint(Expression.Constant(0));
            ExpressionFingerprint fingerprint2 = new FooExpressionFingerprint(Expression.Constant(1));

            // Act
            HashSet<ExpressionFingerprint> set = new HashSet<ExpressionFingerprint>(){
                fingerprint1,
                fingerprint2
            };

            // Assert
            Assert.AreEqual(1, set.Count, "Fingerprints should have been equivalent.");
        }

        [TestMethod]
        public void ConstructorSetsProperties() {
            // Arrange
            Expression expression = Expression.Parameter(typeof(string), "parameter_name");

            // Act
            ExpressionFingerprint fingerprint = new FooExpressionFingerprint(expression);

            // Assert
            Assert.AreEqual(expression.NodeType, fingerprint.NodeType);
            Assert.AreEqual(expression.Type, fingerprint.Type);
        }

        [TestMethod]
        public void EqualsReturnsFalseIfOtherIsNotFingerprint() {
            // Arrange
            Expression expression = Expression.Parameter(typeof(string), "parameter_name");
            ExpressionFingerprint fingerprint = new FooExpressionFingerprint(expression);

            // Act
            bool equalsValue = fingerprint.Equals(5);

            // Assert
            Assert.IsFalse(equalsValue);
        }

        private class FooExpressionFingerprint : ExpressionFingerprint {
            public FooExpressionFingerprint(Expression expression)
                : base(expression) {
            }
            public override Expression ToExpression(ParserContext parserContext) {
                throw new NotImplementedException();
            }
        }

        private class BarExpressionFingerprint : ExpressionFingerprint {
            public BarExpressionFingerprint(Expression expression)
                : base(expression) {
            }
            public override Expression ToExpression(ParserContext parserContext) {
                throw new NotImplementedException();
            }
        }

    }
}
