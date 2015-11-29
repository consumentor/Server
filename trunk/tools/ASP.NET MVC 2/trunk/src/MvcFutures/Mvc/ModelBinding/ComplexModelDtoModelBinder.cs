﻿namespace Microsoft.Web.Mvc.ModelBinding {
    using System;
    using System.Web.Mvc;

    public sealed class ComplexModelDtoModelBinder : IExtensibleModelBinder {

        public bool BindModel(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
            ModelBinderUtil.ValidateBindingContext(bindingContext, typeof(ComplexModelDto), false /* allowNullModel */);

            ComplexModelDto dto = (ComplexModelDto)bindingContext.Model;
            foreach (ModelMetadata propertyMetadata in dto.PropertyMetadata) {
                ExtensibleModelBindingContext propertyBindingContext = new ExtensibleModelBindingContext(bindingContext) {
                    ModelMetadata = propertyMetadata,
                    ModelName = ModelBinderUtil.CreatePropertyModelName(bindingContext.ModelName, propertyMetadata.PropertyName)
                };

                // bind and propagate the values
                IExtensibleModelBinder propertyBinder = bindingContext.ModelBinderProviders.GetBinder(controllerContext, propertyBindingContext);
                if (propertyBinder != null) {
                    if (propertyBinder.BindModel(controllerContext, propertyBindingContext)) {
                        dto.Results[propertyMetadata] = new ComplexModelDtoResult(propertyBindingContext.Model, propertyBindingContext.ValidationNode);
                    }
                    else {
                        dto.Results[propertyMetadata] = null;
                    }
                }
            }

            return true;
        }

    }
}
