﻿namespace Microsoft.Web.Mvc {
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Web.Mvc;

    public class ServerVariablesValueProviderFactory : ValueProviderFactory {

        public override IValueProvider GetValueProvider(ControllerContext controllerContext) {
            NameValueCollection nvc = controllerContext.HttpContext.Request.ServerVariables;
            return new NameValueCollectionValueProvider(nvc, CultureInfo.InvariantCulture);
        }

    }
}
