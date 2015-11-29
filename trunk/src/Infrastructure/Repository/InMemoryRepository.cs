using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Consumentor.ShopGun.Repository
{
    public class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private static readonly Dictionary<Type, object> _storage = new Dictionary<Type, object>();

        public InMemoryRepository()
        { }

        public InMemoryRepository(bool deleteIfExists)
        {
            if (deleteIfExists)
                _storage.Clear();
        }

        private List<T> GetTableForType<T>()
        {
            object untypedTable;
            if (_storage.TryGetValue(typeof(T), out untypedTable) == false)
            {
                List<T> table = CreateTable<T>();
                return table;
            }
            return (List<T>)untypedTable;
        }

        private List<T> CreateTable<T>()
        {
            List<T> table = new List<T>();
            _storage.Add(typeof(T), table);
            return table;
        }


        #region IRepository<TEntity> Members

        public void SetLoadOptions(DataLoadOptions loadOptions)
        {
            throw new NotImplementedException();
        }

        public void ToggleDeferredLoading(bool deferredLoadingEnabled)
        {
            throw new NotImplementedException();
        }

        void IRepository<TEntity>.Add(TEntity entity)
        {
            List<TEntity> table = GetTableForType<TEntity>();
            table.Add(entity);
        }

        void IRepository<TEntity>.Delete(TEntity entity)
        {
            List<TEntity> table = GetTableForType<TEntity>();
            table.Remove(entity);
        }

        void IRepository<TEntity>.Persist()
        {
            //
        }

        void IRepository<TEntity>.MergePersist()
        {
            //
        }

        IEnumerable<TEntity> IRepository<TEntity>.Find(Expression<Func<TEntity, bool>> criteria)
        {
            return GetTableForType<TEntity>().AsQueryable().Where(criteria).ToList();
        }

        TEntity IRepository<TEntity>.FindOne(Expression<Func<TEntity, bool>> criteria)
        {
            IEnumerable<TEntity> foundItems = ((IRepository<TEntity>)this).Find(criteria);
            if (foundItems.Count() != 1)
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The given criteria returns {0} results, exactly one result is expected", foundItems.Count()), "criteria");
            return foundItems.First();
        }

        void IRepository<TEntity>.Delete(IEnumerable<TEntity> items)
        {
            foreach (TEntity item in items)
            {
                ((IRepository<TEntity>)this).Delete(item);
            }
        }

        void IRepository<TEntity>.Add(IEnumerable<TEntity> items)
        {
            foreach (TEntity item in items)
            {
                ((IRepository<TEntity>)this).Add(item);
            }             
        }

        public TDomain FindDomainObject<TDomain>(TDomain domainObject) where TDomain : class
        {
            throw new NotImplementedException();
        }

        public T AttachItem<T>(T item) where T : class
        {
            throw new NotImplementedException();
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}