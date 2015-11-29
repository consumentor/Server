﻿namespace Microsoft.Web.Mvc.ModelBinding {
    using System;
    using System.Web.Mvc;

    public sealed class MutableObjectModelBinderProvider : ModelBinderProvider {

        public override IExtensibleModelBinder GetBinder(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
            ModelBinderUtil.ValidateBindingContext(bindingContext);

            if (!bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName)) {
                // no values to bind
                return null;
            }

            if (bindingContext.ModelType == typeof(ComplexModelDto)) {
                // forbidden type - will cause a stack overflow if we try binding this type
                return null;
            }

            return new MutableObjectModelBinder();
        }

    }
}
