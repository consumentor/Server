namespace Microsoft.Web.Mvc.ModelBinding {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;

    public class ExtensibleModelBindingContext {

        private ModelBinderProviderCollection _modelBinderProviders;
        private ModelMetadata _modelMetadata;
        private string _modelName;
        private ModelStateDictionary _modelState;
        private Dictionary<string, ModelMetadata> _propertyMetadata;
        private ModelValidationNode _validationNode;
        private IValueProvider _valueProvider;

        public ExtensibleModelBindingContext()
            : this(null) {
        }

        // copies certain values that won't change between parent and child objects,
        // e.g. ValueProvider, ModelState
        public ExtensibleModelBindingContext(ExtensibleModelBindingContext bindingContext) {
            if (bindingContext != null) {
                ModelBinderProviders = bindingContext.ModelBinderProviders;
                ModelState = bindingContext.ModelState;
                ValueProvider = bindingContext.ValueProvider;
            }
        }

        public object Model {
            get {
                EnsureModelMetadata();
                return ModelMetadata.Model;
            }
            set {
                EnsureModelMetadata();
                ModelMetadata.Model = value;
            }
        }

        public ModelBinderProviderCollection ModelBinderProviders {
            get {
                if (_modelBinderProviders == null) {
                    _modelBinderProviders = Microsoft.Web.Mvc.ModelBinding.ModelBinderProviders.Providers;
                }
                return _modelBinderProviders;
            }
            set {
                _modelBinderProviders = value;
            }
        }

        public ModelMetadata ModelMetadata {
            get {
                return _modelMetadata;
            }
            set {
                _modelMetadata = value;
            }
        }

        public string ModelName {
            get {
                if (_modelName == null) {
                    _modelName = String.Empty;
                }
                return _modelName;
            }
            set {
                _modelName = value;
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "The containing type is mutable.")]
        public ModelStateDictionary ModelState {
            get {
                if (_modelState == null) {
                    _modelState = new ModelStateDictionary();
                }
                return _modelState;
            }
            set {
                _modelState = value;
            }
        }

        public Type ModelType {
            get {
                EnsureModelMetadata();
                return ModelMetadata.ModelType;
            }
        }

        public IDictionary<string, ModelMetadata> PropertyMetadata {
            get {
                if (_propertyMetadata == null) {
                    _propertyMetadata = ModelMetadata.Properties.ToDictionary(m => m.PropertyName, StringComparer.OrdinalIgnoreCase);
                }

                return _propertyMetadata;
            }
        }

        public ModelValidationNode ValidationNode {
            get {
                if (_validationNode == null) {
                    _validationNode = new ModelValidationNode(ModelMetadata, ModelName);
                }
                return _validationNode;
            }
            set {
                _validationNode = value;
            }
        }

        public IValueProvider ValueProvider {
            get {
                return _valueProvider;
            }
            set {
                _valueProvider = value;
            }
        }

        private void EnsureModelMetadata() {
            if (ModelMetadata == null) {
                throw Error.ModelBindingContext_ModelMetadataMustBeSet();
            }
        }

    }
}
