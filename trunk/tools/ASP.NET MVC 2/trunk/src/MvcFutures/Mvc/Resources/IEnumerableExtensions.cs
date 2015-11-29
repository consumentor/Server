namespace Microsoft.Web.Mvc.Resources {
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public static class IEnumerableExtensions {
        /// <summary>
        /// Convenience API to allow an IEnumerable<T> (such as returned by Linq2Sql) to be serialized by DataContractSerilizer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IEnumerable<T> AsSerializable<T>(this IEnumerable<T> collection) where T : class {
            return new IEnumerableWrapper<T>(collection);
        }

        // This wrapper allows IEnumerable<T> to be serialized by DataContractSerilizer
        // it implements the minimal amount of surface needed for serialization.
        class IEnumerableWrapper<T> : IEnumerable<T>
            where T : class {
            IEnumerable<T> collection;

            // The DataContractSerilizer needs a default constructor to ensure the object can be
            // deserialized. We have a dummy one since we don't actually need deserialization.
            public IEnumerableWrapper() {
                throw new NotImplementedException();
            }

            internal IEnumerableWrapper(IEnumerable<T> collection) {
                this.collection = collection;
            }

            // The DataContractSerilizer needs an Add method to ensure the object can be
            // deserialized. We have a dummy one since we don't actually need deserialization.
            public void Add(T item) {
                throw new NotImplementedException();
            }

            public IEnumerator<T> GetEnumerator() {
                return this.collection.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return ((IEnumerable)this.collection).GetEnumerator();
            }
        }
    }
}
