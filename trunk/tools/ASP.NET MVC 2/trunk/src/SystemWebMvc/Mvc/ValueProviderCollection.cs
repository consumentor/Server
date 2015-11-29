namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class ValueProviderCollection : Collection<IValueProvider>, IValueProvider {

        public ValueProviderCollection() {
        }

        public ValueProviderCollection(IList<IValueProvider> list)
            : base(list) {
        }

        public virtual bool ContainsPrefix(string prefix) {
            return this.Any(vp => vp.ContainsPrefix(prefix));
        }

        public virtual ValueProviderResult GetValue(string key) {
            return (from provider in this
                    let result = provider.GetValue(key)
                    where result != null
                    select result).FirstOrDefault();
        }

        protected override void InsertItem(int index, IValueProvider item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, IValueProvider item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }
            base.SetItem(index, item);
        }

    }
}
