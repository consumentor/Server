namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class ButtonsAndLinkExtensions {
        /// <summary>
        /// Creates a submit button for your form
        /// </summary>
        /// <param name="buttonText">The text for the button face</param>
        public static MvcHtmlString SubmitButton(this HtmlHelper helper, string name) {
            return SubmitButton(helper, name, null, (IDictionary<string, object>)null);
        }

        /// <summary>
        /// Creates a submit button for your form
        /// </summary>
        /// <param name="buttonText">The text for the button face</param>
        public static MvcHtmlString SubmitButton(this HtmlHelper helper, string name, string buttonText) {
            return SubmitButton(helper, name, buttonText, null);
        }

        /// <summary>
        /// Creates a submit button for your form
        /// </summary>
        public static MvcHtmlString SubmitButton(this HtmlHelper helper) {
            return SubmitButton(helper, null, null, (IDictionary<string, object>)null);
        }

        /// <summary>
        /// Creates a submit button for your form
        /// </summary>
        /// <param name="name">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static MvcHtmlString SubmitButton(this HtmlHelper helper, string name, string buttonText, object htmlAttributes) {
            return helper.SubmitButton(name, buttonText, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Creates a submit button for your form
        /// </summary>
        /// <param name="name">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="htmlAttributes">Dictionary of HTML settings</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "Required for Extension Method")]
        public static MvcHtmlString SubmitButton(this HtmlHelper helper, string name, string buttonText, IDictionary<string, object> htmlAttributes) {
            return MvcHtmlString.Create(ButtonBuilder.SubmitButton(name, buttonText, htmlAttributes).ToString(TagRenderMode.SelfClosing));
        }

        /// <summary>
        /// Creates a submit button for your form using an image
        /// </summary>
        /// <param name="name">Name of the button</param>
        /// <param name="imageSrc">The URL for the image</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static MvcHtmlString SubmitImage(this HtmlHelper helper, string name, string imageSrc) {
            return helper.SubmitImage(name, imageSrc, null);
        }

        /// <summary>
        /// Creates a submit button for your form using an image
        /// </summary>
        /// <param name="name">Name of the button</param>
        /// <param name="imageSrc">The URL for the image</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static MvcHtmlString SubmitImage(this HtmlHelper helper, string name, string imageSrc, object htmlAttributes) {
            return helper.SubmitImage(name, imageSrc, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Creates a submit button for your form using an image
        /// </summary>
        /// <param name="htmlName">Name of the button</param>
        /// <param name="imageSrc">The URL for the image</param>
        /// <param name="htmlAttributes">Dictionary of HTML settings</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "The return value is not a regular URL since it may contain ~/ ASP.NET-specific characters")]
        public static MvcHtmlString SubmitImage(this HtmlHelper helper, string name, string imageSrc, IDictionary<string, object> htmlAttributes) {
            if (imageSrc == null) {
                throw new ArgumentNullException("imageSrc");
            }
            
            string resolvedUrl = UrlHelper.GenerateContentUrl(imageSrc, helper.ViewContext.HttpContext);
            return MvcHtmlString.Create(ButtonBuilder.SubmitImage(name, resolvedUrl, htmlAttributes).ToString(TagRenderMode.SelfClosing));
        }

        /// <summary>
        /// A Simple button you can use with javascript
        /// </summary>
        /// <param name="name">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="buttonType">The button type (Button, Submit, or Reset)</param>
        /// <param name="onClickMethod">The method or script routine to call when the button is clicked.</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        public static MvcHtmlString Button(this HtmlHelper helper, string name, string buttonText, HtmlButtonType buttonType) {
            return helper.Button(name, buttonText, buttonType, null, (IDictionary<string, object>)null);
        }

        /// <summary>
        /// A Simple button you can use with javascript
        /// </summary>
        /// <param name="name">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="buttonType">The button type (Button, Submit, or Reset)</param>
        /// <param name="onClickMethod">The method or script routine to call when the button is clicked.</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        public static MvcHtmlString Button(this HtmlHelper helper, string name, string buttonText, HtmlButtonType buttonType, string onClickMethod) {
            return helper.Button(name, buttonText, buttonType, onClickMethod, (IDictionary<string, object>)null);
        }

        /// <summary>
        /// A Simple button you can use with javascript
        /// </summary>
        /// <param name="name">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="buttonType">The button type (Button, Submit, or Reset)</param>
        /// <param name="onClickMethod">The method or script routine to call when the button is clicked.</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "This is an Extension Method and requires this argument")]
        public static MvcHtmlString Button(this HtmlHelper helper, string name, string buttonText, HtmlButtonType buttonType, string onClickMethod, object htmlAttributes) {
            return helper.Button(name, buttonText, buttonType, onClickMethod, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// A Simple button you can use with javascript
        /// </summary>
        /// <param name="name">Name of the button</param>
        /// <param name="buttonText">The text for the button face</param>
        /// <param name="onClickMethod">The method or script routine to call when the button is clicked.</param>
        /// <param name="htmlAttributes">Any attributes you want set on the tag. Use anonymous-type declaration for this: new{class=cssclass}</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "helper", Justification = "This is an Extension Method and requires this argument")]
        public static MvcHtmlString Button(this HtmlHelper helper, string name, string buttonText, HtmlButtonType buttonType, string onClickMethod, IDictionary<string, object> htmlAttributes) {
            return MvcHtmlString.Create(ButtonBuilder.Button(name, buttonText, buttonType, onClickMethod, htmlAttributes).ToString(TagRenderMode.Normal));
        }
    }
}
