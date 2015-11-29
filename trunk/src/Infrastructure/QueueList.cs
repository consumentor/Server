using System;
using System.Collections.Generic;

namespace Consumentor.ShopGun
{
    public class QueueList<T>
        where T : class
    {
        private readonly List<T> _list = new List<T>();

        public T this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }
    
        public void Add(T obj)
        {
            _list.Add(obj);
        }

        public void AddRange(IEnumerable<T> objList)
        {
            _list.AddRange(objList);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public void Remove(T obj)
        {
            _list.Remove(obj);
        }

        public void Enqueue(T obj)
        {
            Add(obj);
        }

        public T Dequeue()
        {
            // Find first, pattern always true
            Predicate<T> pattern = t => true;
            T obj = Find(pattern);    //_list.First(); throws Exception on null
            _list.Remove(obj);
            return obj;
        }

        public T Find(Predicate<T> match)
        {
            return _list.Find(match);
        }

        public IList<T> FindAll(Predicate<T> match)
        {
            return _list.FindAll(match);
        }

        public int IndexOf(T obj)
        {
            return _list.IndexOf(obj);
        }

        public void Sort(Comparison<T> comparison)
        {
            _list.Sort(comparison);
        }       
    }
}