﻿namespace Microsoft.Web.Mvc.ModelBinding {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public sealed class CollectionModelBinderProvider : ModelBinderProvider {

        public override IExtensibleModelBinder GetBinder(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
            ModelBinderUtil.ValidateBindingContext(bindingContext);

            if (bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName)) {
                return CollectionModelBinderUtil.GetGenericBinder(typeof(ICollection<>), typeof(List<>), typeof(CollectionModelBinder<>), bindingContext.ModelMetadata);
            }
            else {
                return null;
            }
        }

    }
}
