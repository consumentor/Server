namespace Microsoft.Web.Mvc.ModelBinding {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public sealed class KeyValuePairModelBinder<TKey, TValue> : IExtensibleModelBinder {

        private ModelMetadataProvider _metadataProvider;

        internal ModelMetadataProvider MetadataProvider {
            get {
                if (_metadataProvider == null) {
                    _metadataProvider = ModelMetadataProviders.Current;
                }
                return _metadataProvider;
            }
            set {
                _metadataProvider = value;
            }
        }

        public bool BindModel(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
            ModelBinderUtil.ValidateBindingContext(bindingContext, typeof(KeyValuePair<TKey, TValue>), true /* allowNullModel */);

            TKey key;
            bool keyBindingSucceeded = KeyValuePairModelBinderUtil.TryBindStrongModel<TKey>(controllerContext, bindingContext, "key", MetadataProvider, out key);

            TValue value;
            bool valueBindingSucceeded = KeyValuePairModelBinderUtil.TryBindStrongModel<TValue>(controllerContext, bindingContext, "value", MetadataProvider, out value);

            if (keyBindingSucceeded && valueBindingSucceeded) {
                bindingContext.Model = new KeyValuePair<TKey, TValue>(key, value);
            }
            return keyBindingSucceeded || valueBindingSucceeded;
        }

    }
}
