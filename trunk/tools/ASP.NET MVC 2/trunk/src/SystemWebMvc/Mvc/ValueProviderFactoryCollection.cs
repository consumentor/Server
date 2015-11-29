namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class ValueProviderFactoryCollection : Collection<ValueProviderFactory> {

        public ValueProviderFactoryCollection() {
        }

        public ValueProviderFactoryCollection(IList<ValueProviderFactory> list)
            : base(list) {
        }

        public IValueProvider GetValueProvider(ControllerContext controllerContext) {
            var valueProviders = from factory in this
                                 let valueProvider = factory.GetValueProvider(controllerContext)
                                 where valueProvider != null
                                 select valueProvider;

            return new ValueProviderCollection(valueProviders.ToList());
        }


        protected override void InsertItem(int index, ValueProviderFactory item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, ValueProviderFactory item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }
            base.SetItem(index, item);
        }

    }
}
