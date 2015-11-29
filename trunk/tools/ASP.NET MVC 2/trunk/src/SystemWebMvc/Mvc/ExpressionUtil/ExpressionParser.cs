namespace System.Web.Mvc.ExpressionUtil {
    using System;
    using System.Linq.Expressions;

    internal static class ExpressionParser {

        public static ParserContext Parse<TModel, TValue>(Expression<Func<TModel, TValue>> expression) {
            ParserContext context = new ParserContext() {
                ModelParameter = expression.Parameters[0]
            };

            Expression body = expression.Body;
            context.Fingerprint = ExpressionFingerprint.Create(body, context);
            return context;
        }

    }
}
