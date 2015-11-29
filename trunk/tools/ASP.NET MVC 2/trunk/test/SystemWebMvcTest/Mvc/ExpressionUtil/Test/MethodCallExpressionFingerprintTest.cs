namespace System.Web.Mvc.ExpressionUtil.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MethodCallExpressionFingerprintTest {

        [TestMethod]
        public void Comparison_DifferByArguments() {
            // fingerprint1 is CALL(String.Contains, PARAM:string, PARAM:string):bool
            // fingerprint2 is CALL(String.Contains, PARAM:string, CONST:string):bool, so not equivalent

            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(string), "model") };
            MethodInfo method = typeof(string).GetMethod("Contains");

            ExpressionFingerprint fingerprint1 = MethodCallExpressionFingerprint.Create(Expression.Call(context.ModelParameter, method, context.ModelParameter), context);
            ExpressionFingerprint fingerprint2 = MethodCallExpressionFingerprint.Create(Expression.Call(context.ModelParameter, method, Expression.Constant("")), context);

            // Act
            bool areEqual = Object.Equals(fingerprint1, fingerprint2);

            // Assert
            Assert.IsFalse(areEqual, "Fingerprints should not have been equivalent - differ by MethodCallExpressionFingerprint.Arguments.");
        }

        [TestMethod]
        public void Comparison_DifferByMethod() {
            // fingerprint1 is CALL(String.ToLower, PARAM:string):string
            // fingerprint1 is CALL(String.ToUpper, PARAM:string):string, so not equivalent

            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(string), "model") };
            MethodInfo toLower = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            MethodInfo toUpper = typeof(string).GetMethod("ToUpper", Type.EmptyTypes);

            ExpressionFingerprint fingerprint1 = MethodCallExpressionFingerprint.Create(Expression.Call(context.ModelParameter, toLower), context);
            ExpressionFingerprint fingerprint2 = MethodCallExpressionFingerprint.Create(Expression.Call(context.ModelParameter, toUpper), context);

            // Act
            bool areEqual = Object.Equals(fingerprint1, fingerprint2);

            // Assert
            Assert.IsFalse(areEqual, "Fingerprints should not have been equivalent - differ by MethodCallExpressionFingerprint.Method.");
        }

        [TestMethod]
        public void Comparison_DifferByTarget() {
            // fingerprint1 is CALL(String.Clone, PARAM:string):object
            // fingerprint2 is CALL(String.Clone, CONST:string):object, so not equivalent

            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(string), "model") };
            MethodInfo method = typeof(string).GetMethod("Clone");

            ExpressionFingerprint fingerprint1 = MethodCallExpressionFingerprint.Create(Expression.Call(context.ModelParameter, method), context);
            ExpressionFingerprint fingerprint2 = MethodCallExpressionFingerprint.Create(Expression.Call(Expression.Constant(""), method), context);

            // Act
            bool areEqual = Object.Equals(fingerprint1, fingerprint2);

            // Assert
            Assert.IsFalse(areEqual, "Fingerprints should not have been equivalent - differ by MethodCallExpressionFingerprint.Target.");
        }

        [TestMethod]
        public void Comparison_EquivalentExpressions() {
            // both fingerprints are CALL(String.Clone, PARAM:string):object, so are equivalent

            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(string), "model") };
            MethodInfo method = typeof(string).GetMethod("Clone");

            ExpressionFingerprint fingerprint1 = MethodCallExpressionFingerprint.Create(Expression.Call(context.ModelParameter, method), context);
            ExpressionFingerprint fingerprint2 = MethodCallExpressionFingerprint.Create(Expression.Call(context.ModelParameter, method), context);

            // Act
            HashSet<ExpressionFingerprint> set = new HashSet<ExpressionFingerprint>() {
                fingerprint1,
                fingerprint2
            };

            // Assert
            Assert.AreEqual(1, set.Count, "Fingerprints should have been equivalent.");
        }

        [TestMethod]
        public void Create_InstanceMethod() {
            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(string), "model") };
            MethodInfo method = typeof(string).GetMethod("Clone");

            MethodCallExpression expression = Expression.Call(context.ModelParameter, method);

            // Act
            MethodCallExpressionFingerprint fingerprint = MethodCallExpressionFingerprint.Create(expression, context);

            // Assert
            Assert.AreEqual(method, fingerprint.Method);
            Assert.AreEqual(ExpressionType.Parameter, fingerprint.Target.NodeType);
            Assert.AreEqual(0, fingerprint.Arguments.Count);
        }

        [TestMethod]
        public void Create_StaticMethod() {
            // Arrange
            ParserContext context = new ParserContext();
            MethodInfo method = typeof(object).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public);

            MethodCallExpression expression = Expression.Call(null, method, Expression.Constant(new object()), Expression.Constant(new object()));

            // Act
            MethodCallExpressionFingerprint fingerprint = MethodCallExpressionFingerprint.Create(expression, context);

            // Assert
            Assert.AreEqual(method, fingerprint.Method);
            Assert.IsNull(fingerprint.Target);
            Assert.AreEqual(2, fingerprint.Arguments.Count);
        }

        [TestMethod]
        public void Create_UnknownArguments() {
            // Arrange
            ParserContext context = new ParserContext();
            MethodInfo method = typeof(object).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public);

            MethodCallExpression expression = Expression.Call(null, method, Expression.Constant(new object()), ExpressionHelper.GetUnknownExpression(typeof(object)));

            // Act
            MethodCallExpressionFingerprint fingerprint = MethodCallExpressionFingerprint.Create(expression, context);

            // Assert
            Assert.IsNull(fingerprint, "Unknown arguments cannot be parsed.");
        }

        [TestMethod]
        public void Create_UnknownTarget() {
            // Arrange
            ParserContext context = new ParserContext();
            MethodInfo method = typeof(string).GetMethod("Clone");

            MethodCallExpression expression = Expression.Call(ExpressionHelper.GetUnknownExpression(typeof(string)), method);

            // Act
            MethodCallExpressionFingerprint fingerprint = MethodCallExpressionFingerprint.Create(expression, context);

            // Assert
            Assert.IsNull(fingerprint, "Unknown target cannot be parsed.");
        }

        [TestMethod]
        public void ToExpression() {
            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(string), "model") };
            MethodInfo method = typeof(string).GetMethod("Intern");
            MethodCallExpression expression = Expression.Call(null, method, context.ModelParameter);
            MethodCallExpressionFingerprint fingerprint = MethodCallExpressionFingerprint.Create(expression, context);

            // Act
            MethodCallExpression result = (MethodCallExpression)fingerprint.ToExpression(context);

            // Assert
            Assert.AreEqual(method, result.Method);
            Assert.IsNull(result.Object);
            Assert.AreEqual(1, result.Arguments.Count);
            Assert.AreEqual(context.ModelParameter, result.Arguments[0]);
        }

    }
}
