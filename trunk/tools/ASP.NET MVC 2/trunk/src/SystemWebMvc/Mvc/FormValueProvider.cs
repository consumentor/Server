namespace System.Web.Mvc {
    using System;
    using System.Collections.Specialized;
    using System.Globalization;

    public sealed class FormValueProvider : NameValueCollectionValueProvider {

        public FormValueProvider(ControllerContext controllerContext)
            : base(controllerContext.HttpContext.Request.Form, CultureInfo.CurrentCulture) {
        }

    }
}
