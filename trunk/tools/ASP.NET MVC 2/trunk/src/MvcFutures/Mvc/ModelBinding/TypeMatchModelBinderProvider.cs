﻿namespace Microsoft.Web.Mvc.ModelBinding {
    using System;
    using System.Web.Mvc;

    // Returns a binder that can extract a ValueProviderResult.RawValue and return it directly.
    [ModelBinderProviderOptions(FrontOfList = true)]
    public sealed class TypeMatchModelBinderProvider : ModelBinderProvider {

        public override IExtensibleModelBinder GetBinder(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
            return (TypeMatchModelBinder.GetCompatibleValueProviderResult(bindingContext) != null)
                ? new TypeMatchModelBinder()
                : null /* no match */;
        }

    }
}
