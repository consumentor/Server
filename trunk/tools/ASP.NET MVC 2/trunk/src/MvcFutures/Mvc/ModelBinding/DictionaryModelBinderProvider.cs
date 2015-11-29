﻿namespace Microsoft.Web.Mvc.ModelBinding {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public sealed class DictionaryModelBinderProvider : ModelBinderProvider {

        public override IExtensibleModelBinder GetBinder(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
            ModelBinderUtil.ValidateBindingContext(bindingContext);

            if (bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName)) {
                return CollectionModelBinderUtil.GetGenericBinder(typeof(IDictionary<,>), typeof(Dictionary<,>), typeof(DictionaryModelBinder<,>), bindingContext.ModelMetadata);
            }
            else {
                return null;
            }
        }

    }
}
