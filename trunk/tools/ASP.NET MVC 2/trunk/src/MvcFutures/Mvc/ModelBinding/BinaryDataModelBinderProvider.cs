namespace Microsoft.Web.Mvc.ModelBinding {
    using System;
    using System.Data.Linq;
    using System.Linq;
    using System.Web.Mvc;

    // This is a single provider that can work with both byte[] and Binary models.
    public sealed class BinaryDataModelBinderProvider : ModelBinderProvider {

        private static readonly ModelBinderProvider[] _providers = new ModelBinderProvider[] {
            new SimpleModelBinderProvider(typeof(byte[]), new ByteArrayExtensibleModelBinder()),
            new SimpleModelBinderProvider(typeof(Binary), new LinqBinaryExtensibleModelBinder())
        };

        public override IExtensibleModelBinder GetBinder(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
            return (from provider in _providers
                    let binder = provider.GetBinder(controllerContext, bindingContext)
                    where binder != null
                    select binder).FirstOrDefault();
        }

        // This is essentially a clone of the ByteArrayModelBinder from core
        private class ByteArrayExtensibleModelBinder : IExtensibleModelBinder {
            public bool BindModel(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
                ModelBinderUtil.ValidateBindingContext(bindingContext);
                ValueProviderResult vpResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

                // case 1: there was no <input ... /> element containing this data
                if (vpResult == null) {
                    return false;
                }

                string base64string = (string)vpResult.ConvertTo(typeof(string));

                // case 2: there was an <input ... /> element but it was left blank
                if (String.IsNullOrEmpty(base64string)) {
                    return false;
                }

                // Future proofing. If the byte array is actually an instance of System.Data.Linq.Binary
                // then we need to remove these quotes put in place by the ToString() method.
                string realValue = base64string.Replace("\"", String.Empty);
                try {
                    bindingContext.Model = ConvertByteArray(Convert.FromBase64String(realValue));
                    return true;
                }
                catch {
                    // corrupt data - just ignore
                    return false;
                }
            }

            protected virtual object ConvertByteArray(byte[] originalModel) {
                return originalModel;
            }
        }

        // This is essentially a clone of the LinqBinaryModelBinder from core
        private class LinqBinaryExtensibleModelBinder : ByteArrayExtensibleModelBinder {
            protected override object ConvertByteArray(byte[] originalModel) {
                return new Binary(originalModel);
            }
        }

    }
}
