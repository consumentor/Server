namespace System.Web.Mvc.ExpressionUtil {
    using System;
    using System.Linq.Expressions;
    using System.Collections.Generic;

    internal class ParserContext {

        public static readonly ParameterExpression HoistedValuesParameter = Expression.Parameter(typeof(object[]), "hoistedValues");

        public ExpressionFingerprint Fingerprint;
        public readonly List<object> HoistedValues = new List<object>();
        public ParameterExpression ModelParameter;

    }
}
