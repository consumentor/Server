namespace System.Web.Mvc.ExpressionUtil.Test {
    using System;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CachedExpressionCompilerTest {

        [TestMethod]
        public void Process_FastTracked() {
            // Identity function

            // Arrange
            Expression<Func<int, int>> expr = model => model;

            // Act
            Func<int, int> func = CachedExpressionCompiler.Process(expr);
            int result = func(42);

            // Assert
            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public void Process_Fingerprinted() {
            // Arrange
            var person1 = new {
                BillingAddress = new {
                    Street1 = "123 Nowhere Ln"
                }
            };

            var person2 = new {
                BillingAddress = new {
                    Street1 = "123 Anywhere St"
                }
            };

            var expr = ToExpression(person1, o => o.BillingAddress.Street1);

            // Act
            var func = CachedExpressionCompiler.Process(expr);
            string result = func(person2);

            // Assert
            Assert.AreEqual("123 Anywhere St", result);
        }

        [TestMethod]
        public void Process_SlowFallback() {
            // Arrange
            Expression<Func<object, bool>> expr = model => model is string;

            // Act
            Func<object, bool> func = CachedExpressionCompiler.Process(expr);
            bool result1 = func(42);
            bool result2 = func("hello, world");

            // Assert
            Assert.IsFalse(result1);
            Assert.IsTrue(result2);
        }

        // for type inference
        private static Expression<Func<TModel, TValue>> ToExpression<TModel, TValue>(TModel model, Expression<Func<TModel, TValue>> expression) {
            return expression;
        }

    }
}
