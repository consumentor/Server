namespace Microsoft.Web.Mvc.ModelBinding {
    using System;
    using System.Web.Mvc;

    public interface IExtensibleModelBinder {
        bool BindModel(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext);
    }
}
