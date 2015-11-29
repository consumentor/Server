namespace Microsoft.Web.Mvc {
    using System;
    using System.Linq.Expressions;
    using System.Web.Mvc;

    public static class NameExtensions {

        public static MvcHtmlString Id(this HtmlHelper html, string name) {
            return MvcHtmlString.Create(html.AttributeEncode(html.ViewData.TemplateInfo.GetFullHtmlFieldId(name)));
        }

        public static MvcHtmlString IdFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression) {
            return Id(html, ExpressionHelper.GetExpressionText(expression));
        }

        public static MvcHtmlString IdForModel(this HtmlHelper html) {
            return Id(html, String.Empty);
        }

        public static MvcHtmlString Name(this HtmlHelper html, string name) {
            return MvcHtmlString.Create(html.AttributeEncode(html.ViewData.TemplateInfo.GetFullHtmlFieldName(name)));
        }

        public static MvcHtmlString NameFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression) {
            return Name(html, ExpressionHelper.GetExpressionText(expression));
        }

        public static MvcHtmlString NameForModel(this HtmlHelper html) {
            return Name(html, String.Empty);
        }

    }
}
