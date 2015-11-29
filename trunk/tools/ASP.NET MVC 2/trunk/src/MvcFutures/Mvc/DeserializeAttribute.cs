namespace Microsoft.Web.Mvc {
    using System;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class DeserializeAttribute : CustomModelBinderAttribute {

        private const SerializationMode _defaultSerializationMode = SerializationMode.Plaintext;

        public DeserializeAttribute()
            : this(_defaultSerializationMode) {
        }

        public DeserializeAttribute(SerializationMode mode) {
            Mode = mode;
        }

        public SerializationMode Mode {
            get;
            private set;
        }

        public override IModelBinder GetBinder() {
            return new DeserializingModelBinder(Mode);
        }

        private sealed class DeserializingModelBinder : IModelBinder {

            private readonly SerializationMode _mode;

            public DeserializingModelBinder(SerializationMode mode) {
                _mode = mode;
            }

            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                if (bindingContext == null) {
                    throw new ArgumentNullException("bindingContext");
                }

                ValueProviderResult vpResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (vpResult == null) {
                    // nothing found
                    return null;
                }

                MvcSerializer serializer = new MvcSerializer();
                string serializedValue = (string)vpResult.ConvertTo(typeof(string));
                return serializer.Deserialize(serializedValue, _mode);
            }

        }

    }
}
