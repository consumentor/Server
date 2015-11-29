namespace Microsoft.Web.Mvc {
    using System;
    using System.Web.Mvc;
    using Microsoft.Web.Resources;

    public static class SerializationExtensions {

        private const SerializationMode _defaultSerializationMode = SerializationMode.Plaintext;

        public static MvcHtmlString Serialize(this HtmlHelper htmlHelper, string name) {
            return Serialize(htmlHelper, name, _defaultSerializationMode);
        }

        public static MvcHtmlString Serialize(this HtmlHelper htmlHelper, string name, SerializationMode mode) {
            return SerializeInternal(htmlHelper, name, null, mode, true /* useViewData */);
        }

        public static MvcHtmlString Serialize(this HtmlHelper htmlHelper, string name, object data) {
            return Serialize(htmlHelper, name, data, _defaultSerializationMode);
        }

        public static MvcHtmlString Serialize(this HtmlHelper htmlHelper, string name, object data, SerializationMode mode) {
            return SerializeInternal(htmlHelper, name, data, mode, false /* useViewData */);
        }

        private static MvcHtmlString SerializeInternal(HtmlHelper htmlHelper, string name, object data, SerializationMode mode, bool useViewData) {
            return SerializeInternal(htmlHelper, name, data, mode, useViewData, new MvcSerializer());
        }

        private static MvcHtmlString SerializeInternal(HtmlHelper htmlHelper, string name, object data, SerializationMode mode, bool useViewData, MvcSerializer serializer) {
            if (htmlHelper == null) {
                throw new ArgumentNullException("htmlHelper");
            }

            if (String.IsNullOrEmpty(name)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "name");
            }

            name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (useViewData) {
                data = htmlHelper.ViewData.Eval(name);
            }

            string serializedData = serializer.Serialize(data, mode);

            TagBuilder builder = new TagBuilder("input");
            builder.Attributes["type"] = "hidden";
            builder.Attributes["name"] = name;
            builder.Attributes["value"] = serializedData;
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }

    }
}
