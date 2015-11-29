using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.ComponentModel;

namespace Consumentor.ShopGun.Domain
{
    public class BindingListSelector<TSource, T> : ListSelector<TSource, T>, IBindingList
    {
        public BindingListSelector(IBindingList source, Func<TSource, T> selector)
            : base(source as IList<TSource>, selector)
        {
            sourceAsBindingList = source;
        }
        public BindingListSelector(IBindingList source, Func<TSource, T> selector, Action<IList<TSource>, T> onAdd, Action<IList<TSource>, T> onRemove)
            : base(source as IList<TSource>, selector, onAdd, onRemove)
        {
            sourceAsBindingList = source;
        }

        protected IBindingList sourceAsBindingList;

        #region IBindingList Members

        public void AddIndex(PropertyDescriptor property)
        {
            sourceAsBindingList.AddIndex(property);
        }

        public object AddNew()
        {
            return sourceAsBindingList.AddNew();
        }

        public bool AllowEdit
        {
            get { return false; }
        }

        public bool AllowNew
        {
            get { return false; }
        }

        public bool AllowRemove
        {
            get { return sourceAsBindingList.AllowRemove; }
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
        }

        public int Find(PropertyDescriptor property, object key)
        {
            return sourceAsBindingList.Find(property, key);
        }

        public bool IsSorted
        {
            get { return sourceAsBindingList.IsSorted; }
        }

        public event ListChangedEventHandler ListChanged
        {
            add
            {
                sourceAsBindingList.ListChanged += value;
            }
            remove
            {
                sourceAsBindingList.ListChanged -= value;
            }
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
            sourceAsBindingList.RemoveIndex(property);
        }

        public void RemoveSort()
        {
            sourceAsBindingList.RemoveSort();
        }

        public ListSortDirection SortDirection
        {
            get { return sourceAsBindingList.SortDirection; }
        }

        public PropertyDescriptor SortProperty
        {
            get { return sourceAsBindingList.SortProperty; }
        }

        public bool SupportsChangeNotification
        {
            get { return sourceAsBindingList.SupportsChangeNotification; }
        }

        public bool SupportsSearching
        {
            get { return sourceAsBindingList.SupportsSearching; }
        }

        public bool SupportsSorting
        {
            get { return false; } // return sourceAsBindingList.SupportsSorting; }
        }

        #endregion
    }

    public static class ListSelectorExtensions
    {
        public static ListSelector<TSource, T> AsListSelector<TSource, T>(this IList<TSource> source, Func<TSource, T> selector)
        {
            return new ListSelector<TSource, T>(source, selector);
        }
        public static ListSelector<TSource, T> AsListSelector<TSource, T>(this IList<TSource> source, Func<TSource, T> selector, Action<IList<TSource>, T> onAdd, Action<IList<TSource>, T> onRemove)
        {
            return new ListSelector<TSource, T>(source, selector, onAdd, onRemove);
        }
        public static BindingListSelector<TSource, T> AsListSelector<TSource, T>(this IBindingList source, Func<TSource, T> selector)
        {
            return new BindingListSelector<TSource, T>(source, selector);
        }
        public static BindingListSelector<TSource, T> AsListSelector<TSource, T>(this IBindingList source, Func<TSource, T> selector, Action<IList<TSource>, T> onAdd, Action<IList<TSource>, T> onRemove)
        {
            return new BindingListSelector<TSource, T>(source, selector, onAdd, onRemove);
        }
    }

    public class ListSelector<TSource, T> : IList<T>, IList
    {
        public ListSelector(IList<TSource> source, Func<TSource, T> selector)
        {
            this.source = source;
            this.selector = selector;
            projection = source.Select(selector);
        }
        public ListSelector(IList<TSource> source, Func<TSource, T> selector, Action<IList<TSource>, T> onAdd, Action<IList<TSource>, T> onRemove)
            : this(source, selector)
        {
            this.onRemove = onRemove;
            this.onAdd = onAdd;
            isReadOnly = false;
        }

        protected IList<TSource> source;
        protected Func<TSource, T> selector;
        protected IEnumerable<T> projection;

        private bool isReadOnly = true;
        protected Action<IList<TSource>, T> onRemove;
        protected Action<IList<TSource>, T> onAdd;

        #region IList<T> Members

        public int IndexOf(T item)
        {
            int i = 0;
            foreach (T t in projection)
            {
                if (t.Equals(item))
                    return i;
                i++;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RemoveAt(int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public T this[int index]
        {
            get
            {
                return selector(source[index]);
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            if (onAdd != null)
                onAdd(source, item);
        }

        public void Clear()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Contains(T item)
        {
            return projection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            //source.CopyTo(projection.ToArray(), arrayIndex);
        }

        public int Count
        {
            get { return source.Count; }
        }

        public bool IsReadOnly
        {
            get { return isReadOnly; }
        }

        public bool Remove(T item)
        {
            if (onRemove != null)
            {
                onRemove(source, item);
                return true;
            }
            else
                return false;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return projection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return projection.GetEnumerator();
        }

        #endregion

        #region IList Members

        int IList.Add(object value)
        {
            Add((T)value);

            return 0;
        }

        void IList.Clear()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool IList.IsFixedSize
        {
            get { return true; }
        }

        bool IList.IsReadOnly
        {
            get { return true; }
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        void IList.RemoveAt(int index)
        {
            Remove(this[index]);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            //CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return this.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }

    public class Singleton<T> where T : class, new()
    {
        private static T defaultInstance = null;
        public static T Default
        {
            get
            {
                if (defaultInstance == null)
                    defaultInstance = new T();
                return defaultInstance;
            }
        }
    }
}
