namespace System.Web.Mvc.ExpressionUtil.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MemberExpressionFingerprintTest {

        [TestMethod]
        public void Comparison_DifferByMember() {
            // fingerprint1 is MEMBER_ACCESS(TimeSpan.Hours, PARAM:TimeSpan):int
            // fingerprint2 is MEMBER_ACCESS(TimeSpan.Minutes, PARAM:TimeSpan):int, so not equivalent

            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(TimeSpan), "model") };

            MemberExpressionFingerprint fingerprint1 = MemberExpressionFingerprint.Create(Expression.MakeMemberAccess(context.ModelParameter, typeof(TimeSpan).GetProperty("Hours")), context);
            MemberExpressionFingerprint fingerprint2 = MemberExpressionFingerprint.Create(Expression.MakeMemberAccess(context.ModelParameter, typeof(TimeSpan).GetProperty("Minutes")), context);

            // Act
            bool areEqual = Object.Equals(fingerprint1, fingerprint2);

            // Assert
            Assert.IsFalse(areEqual, "Fingerprints should not have been equivalent - differ by MemberExpressionFingerprint.Member.");
        }

        [TestMethod]
        public void Comparison_DifferByTarget() {
            // fingerprint1 is MEMBER_ACCESS(TimeSpan.Hours, PARAM:TimeSpan):int
            // fingerprint2 is MEMBER_ACCESS(TimeSpan.Hours, CONST:TimeSpan):int, so not equivalent

            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(TimeSpan), "model") };
            PropertyInfo propInfo = typeof(TimeSpan).GetProperty("Hours");

            MemberExpressionFingerprint fingerprint1 = MemberExpressionFingerprint.Create(Expression.MakeMemberAccess(context.ModelParameter, propInfo), context);
            MemberExpressionFingerprint fingerprint2 = MemberExpressionFingerprint.Create(Expression.MakeMemberAccess(Expression.Constant(TimeSpan.Zero), propInfo), context);

            // Act
            bool areEqual = Object.Equals(fingerprint1, fingerprint2);

            // Assert
            Assert.IsFalse(areEqual, "Fingerprints should not have been equivalent - differ by MemberExpressionFingerprint.Target.");
        }

        [TestMethod]
        public void Comparison_EquivalentExpressions() {
            // both fingerprints are MEMBER_ACCESS(String.Length, PARAM:string):int, so are equivalent

            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(string), "model") };
            PropertyInfo propInfo = typeof(string).GetProperty("Length");

            MemberExpressionFingerprint fingerprint1 = MemberExpressionFingerprint.Create(Expression.MakeMemberAccess(context.ModelParameter, propInfo), context);
            MemberExpressionFingerprint fingerprint2 = MemberExpressionFingerprint.Create(Expression.MakeMemberAccess(context.ModelParameter, propInfo), context);

            // Act
            HashSet<ExpressionFingerprint> set = new HashSet<ExpressionFingerprint>() {
                fingerprint1,
                fingerprint2
            };

            // Assert
            Assert.AreEqual(1, set.Count, "Fingerprints should have been equivalent.");
        }

        [TestMethod]
        public void Create_InstanceMember() {
            // Arrange
            PropertyInfo propInfo = typeof(TimeSpan).GetProperty("Hours");
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(TimeSpan), "model") };
            MemberExpression expression = Expression.MakeMemberAccess(context.ModelParameter, propInfo);

            // Act
            MemberExpressionFingerprint fingerprint = MemberExpressionFingerprint.Create(expression, context);

            // Assert
            Assert.AreEqual(ExpressionType.Parameter, fingerprint.Target.NodeType);
            Assert.AreEqual(propInfo, expression.Member);
        }

        [TestMethod]
        public void Create_StaticMember() {
            // Arrange
            FieldInfo fieldInfo = typeof(TimeSpan).GetField("Zero");
            MemberExpression expression = Expression.MakeMemberAccess(null, fieldInfo);

            // Act
            MemberExpressionFingerprint fingerprint = MemberExpressionFingerprint.Create(expression, new ParserContext());

            // Assert
            Assert.IsNull(fingerprint.Target, "Static member should have null target fingerprint.");
            Assert.AreEqual(fieldInfo, expression.Member);
        }

        [TestMethod]
        public void Create_UnknownTarget() {
            // Arrange
            PropertyInfo propInfo = typeof(TimeSpan).GetProperty("Hours");
            MemberExpression expression = Expression.MakeMemberAccess(ExpressionHelper.GetUnknownExpression(typeof(TimeSpan)), propInfo);

            // Act
            MemberExpressionFingerprint fingerprint = MemberExpressionFingerprint.Create(expression, new ParserContext());

            // Assert
            Assert.IsNull(fingerprint, "Unknown operands cannot be parsed.");
        }

        [TestMethod]
        public void ToExpression() {
            // Arrange
            ParserContext context = new ParserContext() { ModelParameter = Expression.Parameter(typeof(TimeSpan), "model") };
            PropertyInfo propInfo = typeof(TimeSpan).GetProperty("Hours");

            MemberExpression expression = Expression.MakeMemberAccess(context.ModelParameter, propInfo);
            MemberExpressionFingerprint fingerprint = MemberExpressionFingerprint.Create(expression, context);

            // Act
            MemberExpression result = (MemberExpression)fingerprint.ToExpression(context);

            // Assert
            Assert.AreEqual(context.ModelParameter, result.Expression);
            Assert.AreEqual(propInfo, result.Member);
        }

    }
}
