namespace Microsoft.Web.Mvc.ModelBinding {
    using System;
    using System.Web.Mvc;

    public sealed class TypeMatchModelBinder : IExtensibleModelBinder {

        public bool BindModel(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
            ValueProviderResult vpResult = GetCompatibleValueProviderResult(bindingContext);
            if (vpResult == null) {
                return false; // conversion would have failed
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, vpResult);
            object model = vpResult.RawValue;
            ModelBinderUtil.ReplaceEmptyStringWithNull(bindingContext.ModelMetadata, ref model);
            bindingContext.Model = model;

            return true;
        }

        internal static ValueProviderResult GetCompatibleValueProviderResult(ExtensibleModelBindingContext bindingContext) {
            ModelBinderUtil.ValidateBindingContext(bindingContext);

            ValueProviderResult vpResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (vpResult == null) {
                return null; // the value doesn't exist
            }

            if (!TypeHelpers.IsCompatibleObject(bindingContext.ModelType, vpResult.RawValue)) {
                return null; // value is of incompatible type
            }

            return vpResult;
        }

    }
}
