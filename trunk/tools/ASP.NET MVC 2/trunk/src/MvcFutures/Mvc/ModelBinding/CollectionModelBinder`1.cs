namespace Microsoft.Web.Mvc.ModelBinding {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using System.Globalization;
    using System.Collections;

    public class CollectionModelBinder<TElement> : IExtensibleModelBinder {

        // Used when the ValueProvider contains the collection to be bound as multiple elements, e.g. foo[0], foo[1].
        private static List<TElement> BindComplexCollection(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
            string indexPropertyName = ModelBinderUtil.CreatePropertyModelName(bindingContext.ModelName, "index");
            ValueProviderResult vpResultIndex = bindingContext.ValueProvider.GetValue(indexPropertyName);
            IEnumerable<string> indexNames = CollectionModelBinderUtil.GetIndexNamesFromValueProviderResult(vpResultIndex);
            return BindComplexCollectionFromIndexes(controllerContext, bindingContext, indexNames);
        }

        internal static List<TElement> BindComplexCollectionFromIndexes(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext, IEnumerable<string> indexNames) {
            bool indexNamesIsFinite;
            if (indexNames != null) {
                indexNamesIsFinite = true;
            }
            else {
                indexNamesIsFinite = false;
                indexNames = CollectionModelBinderUtil.GetZeroBasedIndexes();
            }

            List<TElement> boundCollection = new List<TElement>();
            foreach (string indexName in indexNames) {
                string fullChildName = ModelBinderUtil.CreateIndexModelName(bindingContext.ModelName, indexName);
                ExtensibleModelBindingContext childBindingContext = new ExtensibleModelBindingContext(bindingContext) {
                    ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(TElement)),
                    ModelName = fullChildName
                };

                object boundValue = null;
                IExtensibleModelBinder childBinder = bindingContext.ModelBinderProviders.GetBinder(controllerContext, childBindingContext);
                if (childBinder != null) {
                    if (childBinder.BindModel(controllerContext, childBindingContext)) {
                        boundValue = childBindingContext.Model;

                        // merge validation up
                        bindingContext.ValidationNode.ChildNodes.Add(childBindingContext.ValidationNode);
                    }
                }
                else {
                    // should we even bother continuing?
                    if (!indexNamesIsFinite) {
                        break;
                    }
                }

                boundCollection.Add(ModelBinderUtil.CastOrDefault<TElement>(boundValue));
            }

            return boundCollection;
        }

        public virtual bool BindModel(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
            ModelBinderUtil.ValidateBindingContext(bindingContext);

            ValueProviderResult vpResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            List<TElement> boundCollection = (vpResult != null)
                ? BindSimpleCollection(controllerContext, bindingContext, vpResult.RawValue, vpResult.Culture)
                : BindComplexCollection(controllerContext, bindingContext);

            bool retVal = CreateOrReplaceCollection(controllerContext, bindingContext, boundCollection);
            return retVal;
        }

        // Used when the ValueProvider contains the collection to be bound as a single element, e.g. the raw value
        // is [ "1", "2" ] and needs to be converted to an int[].
        internal static List<TElement> BindSimpleCollection(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext, object rawValue, CultureInfo culture) {
            if (rawValue == null) {
                return null; // nothing to do
            }

            List<TElement> boundCollection = new List<TElement>();

            object[] rawValueArray = ModelBinderUtil.RawValueToObjectArray(rawValue);
            foreach (object rawValueElement in rawValueArray) {
                ExtensibleModelBindingContext innerBindingContext = new ExtensibleModelBindingContext(bindingContext) {
                    ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(TElement)),
                    ModelName = bindingContext.ModelName,
                    ValueProvider = new ValueProviderCollection() { // aggregate value provider
                        new ElementalValueProvider(bindingContext.ModelName, rawValueElement, culture), // our temporary provider goes at the front of the list
                        bindingContext.ValueProvider
                    }
                };

                object boundValue = null;
                IExtensibleModelBinder childBinder = bindingContext.ModelBinderProviders.GetBinder(controllerContext, innerBindingContext);
                if (childBinder != null) {
                    if (childBinder.BindModel(controllerContext, innerBindingContext)) {
                        boundValue = innerBindingContext.Model;
                        bindingContext.ValidationNode.ChildNodes.Add(innerBindingContext.ValidationNode);
                    }
                }
                boundCollection.Add(ModelBinderUtil.CastOrDefault<TElement>(boundValue));
            }

            return boundCollection;
        }

        // Extensibility point that allows the bound collection to be manipulated or transformed before
        // being returned from the binder.
        protected virtual bool CreateOrReplaceCollection(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext, IList<TElement> newCollection) {
            CollectionModelBinderUtil.CreateOrReplaceCollection(bindingContext, newCollection, () => new List<TElement>());
            return true;
        }

    }
}
