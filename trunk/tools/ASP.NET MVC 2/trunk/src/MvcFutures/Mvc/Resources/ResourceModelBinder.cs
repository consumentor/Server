namespace Microsoft.Web.Mvc.Resources {
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Mime;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.Web.Resources;
    using System.Collections.Generic;

    /// <summary>
    /// ModelBinder implementation that augments the inner model binder with support for binding to other formats -
    /// XML and JSON by default.
    /// </summary>
    public class ResourceModelBinder : IModelBinder {
        IModelBinder inner;

        /// <summary>
        /// Wraps the ModelBinders.Binders.DefaultBinder
        /// </summary>
        public ResourceModelBinder()
            : this(ModelBinders.Binders.DefaultBinder) {
        }

        public ResourceModelBinder(IModelBinder inner) {
            this.inner = inner;
        }

        class MyDefaultModelBinder : DefaultModelBinder {
            public void CallOnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                base.OnModelUpdated(controllerContext, bindingContext);
            }
            public bool CallIsModelValid(ModelBindingContext bindingContext) {
                return DefaultModelBinder.IsModelValid(bindingContext);
            }
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            if (WebApiEnabledAttribute.IsDefined(controllerContext.Controller)) {
                if (!controllerContext.RouteData.Values.ContainsKey(bindingContext.ModelName) && controllerContext.HttpContext.Request.HasBody()) {
                    ContentType requestFormat = controllerContext.RequestContext.GetRequestFormat();
                    object model;
                    if (TryBindModel(controllerContext, bindingContext, requestFormat, out model)) {
                        bindingContext.ModelMetadata.Model = model;
                        MyDefaultModelBinder dmb = new MyDefaultModelBinder();
                        dmb.CallOnModelUpdated(controllerContext, bindingContext);
                        if (!dmb.CallIsModelValid(bindingContext)) {
                            List<ModelError> details = new List<ModelError>();
                            foreach (ModelState ms in bindingContext.ModelState.Values) {
                                foreach (ModelError me in ms.Errors) {
                                    details.Add(me);
                                }
                            }
                            HttpException failure = new HttpException((int)HttpStatusCode.ExpectationFailed, "Invalid Model");
                            failure.Data["details"] = details;
                            throw failure;
                        }
                        return model;
                    }
                    throw new HttpException((int)HttpStatusCode.UnsupportedMediaType, string.Format(CultureInfo.CurrentUICulture, MvcResources.Resources_UnsupportedMediaType, (requestFormat == null ? string.Empty : requestFormat.MediaType)));
                }
            }
            return this.inner.BindModel(controllerContext, bindingContext);
        }

        public bool TryBindModel(ControllerContext controllerContext, ModelBindingContext bindingContext, ContentType requestFormat, out object model) {
            if (requestFormat != null && string.Compare(requestFormat.MediaType, FormatManager.UrlEncoded, StringComparison.OrdinalIgnoreCase) == 0) {
                model = this.inner.BindModel(controllerContext, bindingContext);
                return true;
            }
            if (!FormatManager.Current.TryDeserialize(controllerContext, bindingContext, requestFormat, out model)) {
                model = null;
                return false;
            }
            return true;
        }
    }
}
