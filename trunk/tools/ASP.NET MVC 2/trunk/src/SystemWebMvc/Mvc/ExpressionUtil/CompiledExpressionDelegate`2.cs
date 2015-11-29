namespace System.Web.Mvc.ExpressionUtil {
    using System;

    internal delegate TValue CompiledExpressionDelegate<TModel, TValue>(TModel model, object[] hoistedValues);

}
