using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq.Expressions;


namespace Consumentor.ShopGun.Repository
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        
        void Add(TEntity entity);
        void Delete(TEntity entity);
        void Persist();
        void MergePersist();

        void SetLoadOptions(DataLoadOptions loadOptions);
        void ToggleDeferredLoading(bool deferredLoadingEnabled);

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> criteria);
        /// <summary>
        /// If more than one item match the criteria an ArgumentException is thrown
        /// </summary>
        /// <param name="criteria">A lambda expression that will find one object</param>
        /// <returns></returns>
        TEntity FindOne(Expression<Func<TEntity, bool>> criteria);
        // Allowing the method below allow us to write "select d from xx.Find<Defects> where ....
        //But that will litter our code with code that would be transformed into SQL, do we want that?
        //IQueryable<T> Find<T>() where T : class;

        void Delete(IEnumerable<TEntity> items);
        void Add(IEnumerable<TEntity> items);

        TDomain FindDomainObject<TDomain>(TDomain domainObject) where TDomain : class;
        T AttachItem<T>(T item) where T : class;
    }
}
