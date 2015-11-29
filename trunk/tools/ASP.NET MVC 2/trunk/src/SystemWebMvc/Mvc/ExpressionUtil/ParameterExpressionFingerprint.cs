namespace System.Web.Mvc.ExpressionUtil {
    using System;
    using System.Linq.Expressions;

    // ParameterExpression fingerprint class
    // Specifically, instances of this type represent the model parameter
    internal sealed class ParameterExpressionFingerprint : ExpressionFingerprint {

        private ParameterExpressionFingerprint(ParameterExpression expression)
            : base(expression) {
        }

        public static ParameterExpressionFingerprint Create(ParameterExpression expression, ParserContext parserContext) {
            if (expression == parserContext.ModelParameter) {
                return new ParameterExpressionFingerprint(expression);
            }
            else {
                // degenerate case - uncaptured parameter expression passed into the system
                return null;
            }
        }

        public override Expression ToExpression(ParserContext parserContext) {
            // The only time an instance of this class exists is if it represents the actual model parameter,
            // so just return it directly.
            return parserContext.ModelParameter;
        }

    }
}
