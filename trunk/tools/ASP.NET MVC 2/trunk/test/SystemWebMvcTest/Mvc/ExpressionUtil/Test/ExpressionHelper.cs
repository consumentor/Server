namespace System.Web.Mvc.ExpressionUtil.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class ExpressionHelper {

        public static Expression GetUnknownExpression() {
            return GetUnknownExpression(typeof(object));
        }

        public static Expression GetUnknownExpression(Type type) {
            return new CustomExpression(type);
        }

        // the Expression(ExpressionType, Type) constructor is obsolete in .NET 4, which should be fine since
        // much of the expression tree traversal logic can be moved to ExpressionVisitor
        private sealed class CustomExpression : Expression {
            public CustomExpression(Type type)
                : base((ExpressionType)(-1), type) {
            }
        }

    }
}
