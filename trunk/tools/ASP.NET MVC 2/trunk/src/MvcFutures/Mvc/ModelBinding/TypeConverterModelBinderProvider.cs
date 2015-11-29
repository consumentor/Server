namespace Microsoft.Web.Mvc.ModelBinding {
    using System;
    using System.ComponentModel;
    using System.Web.Mvc;

    // Returns a binder that can perform conversions using a .NET TypeConverter.
    public sealed class TypeConverterModelBinderProvider : ModelBinderProvider {

        public override IExtensibleModelBinder GetBinder(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
            ModelBinderUtil.ValidateBindingContext(bindingContext);

            ValueProviderResult vpResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (vpResult == null) {
                return null; // no value to convert
            }

            if (!TypeDescriptor.GetConverter(bindingContext.ModelType).CanConvertFrom(typeof(string))) {
                return null; // this type cannot be converted
            }

            return new TypeConverterModelBinder();
        }

    }
}
