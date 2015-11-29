namespace Microsoft.Web.Mvc.AspNet4.Html {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;

    // This class contains definitions of single "super" HTML helper methods, which rely on
    // CLR 4's default values for method parameters to make them more consumable. Methods
    // which previously took an HTML attributes object/dictionary now have their legal
    // attribute values all available as optional parameters. Some attributes are only
    // applicable for some DTDs; deprecated attributes (like "align" on input) were
    // specifically excluded.
    //
    // Since htmlAttributes was very often the last parameter to HTML helper methods,
    // converting to these new syntaxes should be as simple as converting your anonymous
    // object htmlAttributes collections into optional parameters.
    //
    // Where there were two overloads for route values (anonymous object and dictionary),
    // there is only a single overload now, which takes type object. If what you pass is
    // a dictionary of the correct type, then we'll use that; otherwise, we'll assume it's
    // an anonymous object and create the dictionary for you. This should make it simple
    // to port methods using route values, as they should just continue to work as before.
    //
    // Some HTML helpers did not take HTML attributes parameters. They are recreated here
    // so that the user does not have to import both System.Web.Mvc.Html as well as this
    // namespace, since the purpose of these methods is to get rid of all the overloads
    // for the built-in HTML helpers.
    //
    // The legal attribute values were derived from: http://www.w3schools.com/tags/

    public static class HtmlHelperExtensions {
        // ChildActionExtensions

        public static MvcHtmlString Action(this HtmlHelper htmlHelper, string actionName, string controllerName = null, object routeValues = null) {
            return ChildActionExtensions.Action(
                htmlHelper,
                actionName,
                controllerName,
                routeValues as IDictionary<string, object> ?? new RouteValueDictionary(routeValues)
            );
        }

        public static void RenderAction(this HtmlHelper htmlHelper, string actionName, string controllerName = null, object routeValues = null) {
            ChildActionExtensions.RenderAction(
                htmlHelper,
                actionName,
                controllerName,
                routeValues as IDictionary<string, object> ?? new RouteValueDictionary(routeValues)
            );
        }

        // DisplayExtensions

        public static MvcHtmlString Display(this HtmlHelper htmlHelper, string expression, string templateName = null, string htmlFieldName = null) {
            return DisplayExtensions.Display(htmlHelper, expression, templateName, htmlFieldName);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DisplayFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string templateName = null, string htmlFieldName = null) {
            return DisplayExtensions.DisplayFor(htmlHelper, expression, templateName, htmlFieldName);
        }

        public static MvcHtmlString DisplayForModel(this HtmlHelper htmlHelper, string templateName = null, string htmlFieldName = null) {
            return DisplayExtensions.DisplayForModel(htmlHelper, templateName, htmlFieldName);
        }

        // DisplayTextExtensions

        public static MvcHtmlString DisplayText(this HtmlHelper htmlHelper, string name) {
            return DisplayTextExtensions.DisplayText(htmlHelper, name);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DisplayTextFor<TModel, TResult>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression) {
            return DisplayTextExtensions.DisplayTextFor(htmlHelper, expression);
        }

        // EditorExtensions

        public static MvcHtmlString Editor(this HtmlHelper htmlHelper, string expression, string templateName = null, string htmlFieldName = null) {
            return EditorExtensions.Editor(htmlHelper, expression, templateName, htmlFieldName);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString EditorFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string templateName = null, string htmlFieldName = null) {
            return EditorExtensions.EditorFor(htmlHelper, expression, templateName, htmlFieldName);
        }

        public static MvcHtmlString EditorForModel(this HtmlHelper htmlHelper, string templateName = null, string htmlFieldName = null) {
            return EditorExtensions.EditorForModel(htmlHelper, templateName, htmlFieldName);
        }

        // FormExtensions

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, string actionName = null, string controllerName = null, object routeValues = null, FormMethod method = FormMethod.Post, string accept = null, string acceptCharset = null, string cssClass = null, string dir = null, string encType = null, string id = null, string lang = null, string name = null, string style = null, string title = null) {
            return FormExtensions.BeginForm(
                htmlHelper,
                actionName,
                controllerName,
                routeValues as RouteValueDictionary ?? new RouteValueDictionary(routeValues),
                method,
                FormAttributes(accept, acceptCharset, cssClass, dir, encType, id, lang, name, style, title)
            );
        }

        public static MvcForm BeginRouteForm(this HtmlHelper htmlHelper, string routeName, RouteValueDictionary routeValues = null, FormMethod method = FormMethod.Post, string accept = null, string acceptCharset = null, string cssClass = null, string dir = null, string encType = null, string id = null, string lang = null, string name = null, string style = null, string title = null) {
            return FormExtensions.BeginRouteForm(
                htmlHelper,
                routeName,
                routeValues as RouteValueDictionary ?? new RouteValueDictionary(routeValues),
                method,
                FormAttributes(accept, acceptCharset, cssClass, dir, encType, id, lang, name, style, title)
            );
        }

        public static void EndForm(this HtmlHelper htmlHelper) {
            FormExtensions.EndForm(htmlHelper);
        }

        // InputExtensions

        public static MvcHtmlString CheckBox(this HtmlHelper htmlHelper, string name, bool? isChecked = null, string cssClass = null, string dir = null, bool disabled = false, string id = null, string lang = null, int? maxLength = null, bool readOnly = false, int? size = null, string style = null, int? tabIndex = null, string title = null) {
            var htmlAttributes = InputAttributes(cssClass, dir, disabled, id, lang, maxLength, readOnly, size, style, tabIndex, title);

            return isChecked.HasValue
                ? InputExtensions.CheckBox(htmlHelper, name, isChecked.Value, htmlAttributes)
                : InputExtensions.CheckBox(htmlHelper, name, htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString CheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, string cssClass = null, string dir = null, bool disabled = false, string id = null, string lang = null, int? maxLength = null, bool readOnly = false, int? size = null, string style = null, int? tabIndex = null, string title = null) {
            return InputExtensions.CheckBoxFor(
                htmlHelper,
                expression,
                InputAttributes(cssClass, dir, disabled, id, lang, maxLength, readOnly, size, style, tabIndex, title)
            );
        }

        public static MvcHtmlString Hidden(this HtmlHelper htmlHelper, string name, object value = null, string cssClass = null, string id = null, string style = null) {
            return InputExtensions.Hidden(htmlHelper, name, value, Attributes(cssClass, id, style));
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString HiddenFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string cssClass = null, string id = null, string style = null) {
            return InputExtensions.HiddenFor(htmlHelper, expression, Attributes(cssClass, id, style));
        }

        public static MvcHtmlString Password(this HtmlHelper htmlHelper, string name, object value = null, string cssClass = null, string dir = null, bool disabled = false, string id = null, string lang = null, int? maxLength = null, bool readOnly = false, int? size = null, string style = null, int? tabIndex = null, string title = null) {
            return InputExtensions.Password(
                htmlHelper,
                name,
                value,
                InputAttributes(cssClass, dir, disabled, id, lang, maxLength, readOnly, size, style, tabIndex, title)
            );
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString PasswordFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string cssClass = null, bool disabled = false, string dir = null, string id = null, string lang = null, int? maxLength = null, bool readOnly = false, int? size = null, string style = null, int? tabIndex = null, string title = null) {
            return InputExtensions.PasswordFor(
                htmlHelper,
                expression,
                InputAttributes(cssClass, dir, disabled, id, lang, maxLength, readOnly, size, style, tabIndex, title)
            );
        }

        public static MvcHtmlString RadioButton(this HtmlHelper htmlHelper, string name, object value, bool? isChecked = null, string cssClass = null, string dir = null, bool disabled = false, string id = null, string lang = null, int? maxLength = null, bool readOnly = false, int? size = null, string style = null, int? tabIndex = null, string title = null) {
            var htmlAttributes = InputAttributes(cssClass, dir, disabled, id, lang, maxLength, readOnly, size, style, tabIndex, title);

            return isChecked.HasValue
                ? InputExtensions.RadioButton(htmlHelper, name, value, isChecked.Value, htmlAttributes)
                : InputExtensions.RadioButton(htmlHelper, name, value, htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value, string cssClass = null, string dir = null, bool disabled = false, string id = null, string lang = null, int? maxLength = null, bool readOnly = false, int? size = null, string style = null, int? tabIndex = null, string title = null) {
            return InputExtensions.RadioButtonFor(
                htmlHelper,
                expression,
                InputAttributes(cssClass, dir, disabled, id, lang, maxLength, readOnly, size, style, tabIndex, title)
            );
        }

        public static MvcHtmlString TextBox(this HtmlHelper htmlHelper, string name, object value = null, string cssClass = null, string dir = null, bool disabled = false, string id = null, string lang = null, int? maxLength = null, bool readOnly = false, int? size = null, string style = null, int? tabIndex = null, string title = null) {
            return InputExtensions.TextBox(
                htmlHelper,
                name,
                value,
                InputAttributes(cssClass, dir, disabled, id, lang, maxLength, readOnly, size, style, tabIndex, title)
            );
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string cssClass = null, string dir = null, bool disabled = false, string id = null, string lang = null, int? maxLength = null, bool readOnly = false, int? size = null, string style = null, int? tabIndex = null, string title = null) {
            return InputExtensions.TextBoxFor(
                htmlHelper,
                expression,
                InputAttributes(cssClass, dir, disabled, id, lang, maxLength, readOnly, size, style, tabIndex, title)
            );
        }

        // LabelExtensions

        public static MvcHtmlString Label(this HtmlHelper htmlHelper, string expression) {
            return LabelExtensions.Label(htmlHelper, expression);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression) {
            return LabelExtensions.LabelFor(htmlHelper, expression);
        }

        public static MvcHtmlString LabelForModel(this HtmlHelper htmlHelper) {
            return LabelExtensions.LabelForModel(htmlHelper);
        }

        // LinkExtensions

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName = null, string protocol = null, string hostName = null, string fragment = null, object routeValues = null, string accessKey = null, string charset = null, string coords = null, string cssClass = null, string dir = null, string hrefLang = null, string id = null, string lang = null, string name = null, string rel = null, string rev = null, string shape = null, string style = null, string target = null, string title = null) {
            return LinkExtensions.ActionLink(
                htmlHelper,
                linkText,
                actionName,
                controllerName,
                protocol,
                hostName,
                fragment,
                routeValues as RouteValueDictionary ?? new RouteValueDictionary(routeValues),
                AnchorAttributes(accessKey, charset, coords, cssClass, dir, hrefLang, id, lang, name, rel, rev, shape, style, target, title)
            );
        }

        public static MvcHtmlString RouteLink(this HtmlHelper htmlHelper, string linkText, string routeName, string protocol = null, string hostName = null, string fragment = null, object routeValues = null, string accessKey = null, string charset = null, string coords = null, string cssClass = null, string dir = null, string hrefLang = null, string id = null, string lang = null, string name = null, string rel = null, string rev = null, string shape = null, string style = null, string target = null, string title = null) {
            return LinkExtensions.RouteLink(
                htmlHelper,
                linkText,
                routeName,
                protocol,
                hostName,
                fragment,
                routeValues as RouteValueDictionary ?? new RouteValueDictionary(routeValues),
                AnchorAttributes(accessKey, charset, coords, cssClass, dir, hrefLang, id, lang, name, rel, rev, shape, style, target, title)
            );
        }

        // PartialExtensions

        public static MvcHtmlString Partial(this HtmlHelper htmlHelper, string partialViewName, object model = null, ViewDataDictionary viewData = null) {
            return PartialExtensions.Partial(
                htmlHelper,
                partialViewName,
                model,
                viewData ?? htmlHelper.ViewData
            );
        }

        // RenderPartialExtensions

        public static void RenderPartial(this HtmlHelper htmlHelper, string partialViewName, object model = null, ViewDataDictionary viewData = null) {
            RenderPartialExtensions.RenderPartial(
                htmlHelper,
                partialViewName,
                model,
                viewData ?? htmlHelper.ViewData
            );
        }

        // SelectExtensions

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList = null, string optionLabel = null, string cssClass = null, string dir = null, bool disabled = false, string id = null, string lang = null, int? size = null, string style = null, int? tabIndex = null, string title = null) {
            return SelectExtensions.DropDownList(
                htmlHelper,
                name,
                selectList,
                optionLabel,
                SelectAttributes(cssClass, dir, disabled, id, lang, size, style, tabIndex, title)
            );
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList = null, string optionLabel = null, string cssClass = null, string dir = null, bool disabled = false, string id = null, string lang = null, int? size = null, string style = null, int? tabIndex = null, string title = null) {
            return SelectExtensions.DropDownListFor(
                htmlHelper,
                expression,
                selectList,
                optionLabel,
                SelectAttributes(cssClass, dir, disabled, id, lang, size, style, tabIndex, title)
            );
        }

        public static MvcHtmlString ListBox(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList = null, string cssClass = null, string dir = null, bool disabled = false, string id = null, string lang = null, int? size = null, string style = null, int? tabIndex = null, string title = null) {
            return SelectExtensions.ListBox(
                htmlHelper,
                name,
                selectList,
                SelectAttributes(cssClass, dir, disabled, id, lang, size, style, tabIndex, title)
            );
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString ListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList = null, string cssClass = null, string dir = null, bool disabled = false, string id = null, string lang = null, int? size = null, string style = null, int? tabIndex = null, string title = null) {
            return SelectExtensions.ListBoxFor(
                htmlHelper,
                expression,
                selectList,
                SelectAttributes(cssClass, dir, disabled, id, lang, size, style, tabIndex, title)
            );
        }

        // TextAreaExtensions

        public static MvcHtmlString TextArea(this HtmlHelper htmlHelper, string name, string value = null, string accessKey = null, string cssClass = null, int? cols = null, string dir = null, bool disabled = false, string id = null, string lang = null, bool readOnly = false, int? rows = null, string style = null, int? tabIndex = null, string title = null) {
            return TextAreaExtensions.TextArea(
                htmlHelper,
                name,
                value,
                TextAreaAttributes(accessKey, cssClass, cols, dir, disabled, id, lang, readOnly, rows, style, tabIndex, title)
            );
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string accessKey = null, string cssClass = null, int? cols = null, string dir = null, bool disabled = false, string id = null, string lang = null, bool readOnly = false, int? rows = null, string style = null, int? tabIndex = null, string title = null) {
            return TextAreaExtensions.TextAreaFor(
                htmlHelper,
                expression,
                TextAreaAttributes(accessKey, cssClass, cols, dir, disabled, id, lang, readOnly, rows, style, tabIndex, title)
            );
        }

        // ValidationExtensions

        public static void Validate(this HtmlHelper htmlHelper, string modelName) {
            ValidationExtensions.Validate(htmlHelper, modelName);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static void ValidateFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) {
            ValidationExtensions.ValidateFor(htmlHelper, expression);
        }

        public static MvcHtmlString ValidationMessage(this HtmlHelper htmlHelper, string modelName, string validationMessage = null, string cssClass = null, string dir = null, string id = null, string lang = null, string style = null, string title = null) {
            return ValidationExtensions.ValidationMessage(
                htmlHelper,
                modelName,
                validationMessage,
                SpanAttributes(cssClass, dir, id, lang, style, title)
            );
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string validationMessage = null, string cssClass = null, string dir = null, string id = null, string lang = null, string style = null, string title = null) {
            return ValidationExtensions.ValidationMessageFor(
                htmlHelper,
                expression,
                validationMessage,
                SpanAttributes(cssClass, dir, id, lang, style, title)
            );
        }

        // NOTE: If you're using VS 2010 Beta 2 with MVC 2 Preview 2, then you will need to
        // use the second version, without support for excludePropertyErrors. Just flip the
        // true to false.

#if true
        public static MvcHtmlString ValidationSummary(this HtmlHelper htmlHelper, string message = null, bool excludePropertyErrors = false, string cssClass = null, string dir = null, string id = null, string lang = null, string style = null, string title = null) {
            return ValidationExtensions.ValidationSummary(
                htmlHelper,
                excludePropertyErrors,
                message,
                SpanAttributes(cssClass, dir, id, lang, style, title)
            );
        }
#else
        public static MvcHtmlString ValidationSummary(this HtmlHelper htmlHelper, string message = null, string cssClass = null, string dir = null, string id = null, string lang = null, string style = null, string title = null) {
            return ValidationExtensions.ValidationSummary(
                htmlHelper,
                message,
                SpanAttributes(cssClass, dir, id, lang, style, title)
            );
        }
#endif

        // Helper methods

        private static void AddOptional(this IDictionary<string, object> dictionary, string key, bool value) {
            if (value) {
                dictionary[key] = key;
            }
        }

        private static void AddOptional(this IDictionary<string, object> dictionary, string key, object value) {
            if (value != null) {
                dictionary[key] = value;
            }
        }

        private static IDictionary<string, object> Attributes(string cssClass, string id, string style) {
            var htmlAttributes = new RouteValueDictionary();

            htmlAttributes.AddOptional("class", cssClass);
            htmlAttributes.AddOptional("id", id);
            htmlAttributes.AddOptional("style", style);

            return htmlAttributes;
        }

        private static IDictionary<string, object> AnchorAttributes(string accessKey, string charset, string coords, string cssClass, string dir, string hrefLang, string id, string lang, string name, string rel, string rev, string shape, string style, string target, string title) {
            var htmlAttributes = Attributes(cssClass, id, style);

            htmlAttributes.AddOptional("accesskey", accessKey);
            htmlAttributes.AddOptional("charset", charset);
            htmlAttributes.AddOptional("coords", coords);
            htmlAttributes.AddOptional("dir", dir);
            htmlAttributes.AddOptional("hreflang", hrefLang);
            htmlAttributes.AddOptional("lang", lang);
            htmlAttributes.AddOptional("name", name);
            htmlAttributes.AddOptional("rel", rel);
            htmlAttributes.AddOptional("rev", rev);
            htmlAttributes.AddOptional("shape", shape);
            htmlAttributes.AddOptional("target", target);
            htmlAttributes.AddOptional("title", title);

            return htmlAttributes;
        }

        private static IDictionary<string, object> FormAttributes(string accept, string acceptCharset, string cssClass, string dir, string encType, string id, string lang, string name, string style, string title) {
            var htmlAttributes = Attributes(cssClass, id, style);

            htmlAttributes.AddOptional("accept", accept);
            htmlAttributes.AddOptional("accept-charset", acceptCharset);
            htmlAttributes.AddOptional("dir", dir);
            htmlAttributes.AddOptional("enctype", encType);
            htmlAttributes.AddOptional("lang", lang);
            htmlAttributes.AddOptional("name", name);
            htmlAttributes.AddOptional("title", title);

            return htmlAttributes;
        }

        private static IDictionary<string, object> InputAttributes(string cssClass, string dir, bool disabled, string id, string lang, int? maxLength, bool readOnly, int? size, string style, int? tabIndex, string title) {
            var htmlAttributes = Attributes(cssClass, id, style);

            htmlAttributes.AddOptional("dir", dir);
            htmlAttributes.AddOptional("disabled", disabled);
            htmlAttributes.AddOptional("lang", lang);
            htmlAttributes.AddOptional("maxlength", maxLength);
            htmlAttributes.AddOptional("readonly", readOnly);
            htmlAttributes.AddOptional("size", size);
            htmlAttributes.AddOptional("tabindex", tabIndex);
            htmlAttributes.AddOptional("title", title);

            return htmlAttributes;
        }

        private static IDictionary<string, object> SelectAttributes(string cssClass, string dir, bool disabled, string id, string lang, int? size, string style, int? tabIndex, string title) {
            var htmlAttributes = Attributes(cssClass, id, style);

            htmlAttributes.AddOptional("dir", dir);
            htmlAttributes.AddOptional("disabled", disabled);
            htmlAttributes.AddOptional("lang", lang);
            htmlAttributes.AddOptional("size", size);
            htmlAttributes.AddOptional("tabindex", tabIndex);
            htmlAttributes.AddOptional("title", title);

            return htmlAttributes;
        }

        private static IDictionary<string, object> SpanAttributes(string cssClass, string dir, string id, string lang, string style, string title) {
            var htmlAttributes = Attributes(cssClass, id, style);

            htmlAttributes.AddOptional("dir", dir);
            htmlAttributes.AddOptional("lang", lang);
            htmlAttributes.AddOptional("title", title);

            return htmlAttributes;
        }

        private static IDictionary<string, object> TextAreaAttributes(string accessKey, string cssClass, int? cols, string dir, bool disabled, string id, string lang, bool readOnly, int? rows, string style, int? tabIndex, string title) {
            var htmlAttributes = Attributes(cssClass, id, style);

            htmlAttributes.AddOptional("accesskey", accessKey);
            htmlAttributes.AddOptional("cols", cols);
            htmlAttributes.AddOptional("dir", dir);
            htmlAttributes.AddOptional("disabled", disabled);
            htmlAttributes.AddOptional("lang", lang);
            htmlAttributes.AddOptional("readonly", readOnly);
            htmlAttributes.AddOptional("rows", rows);
            htmlAttributes.AddOptional("style", style);
            htmlAttributes.AddOptional("tabindex", tabIndex);
            htmlAttributes.AddOptional("title", title);

            return htmlAttributes;
        }

    }
}
