﻿namespace Microsoft.Web.Mvc.ModelBinding {
    using System;

    public static class ModelBinderProviders {

        private static readonly ModelBinderProviderCollection _providers = CreateDefaultCollection();

        public static ModelBinderProviderCollection Providers {
            get {
                return _providers;
            }
        }

        private static ModelBinderProviderCollection CreateDefaultCollection() {
            return new ModelBinderProviderCollection() {
                new TypeMatchModelBinderProvider(),
                new BinaryDataModelBinderProvider(),
                new KeyValuePairModelBinderProvider(),
                new ComplexModelDtoModelBinderProvider(),
                new ArrayModelBinderProvider(),
                new DictionaryModelBinderProvider(),
                new CollectionModelBinderProvider(),
                new TypeConverterModelBinderProvider(),
                new MutableObjectModelBinderProvider()
            };
        }

    }
}
