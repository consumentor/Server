namespace Microsoft.Web.Mvc.ModelBinding {
    using System;
    using System.Web.Mvc;

    public abstract class ModelBinderProvider {
        public abstract IExtensibleModelBinder GetBinder(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext);
    }
}
