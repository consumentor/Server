using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Xml;
using Castle.Core.Logging;
using Consumentor.ShopGun.Log;

namespace Consumentor.ShopGun.Repository
{
    public class Repository<TEntity> : RepositoryBase, IRepository<TEntity> where TEntity : class
    {
        public ILogger Log { get; set; }

        public Repository(DataContext context)
            : base(context)
        {
            //context.Log = new DataContextLog();
        }

        /// <summary>
        /// Use this property if you want to query the database
        /// </summary>
        protected IQueryable<TEntity> RepositoryQuery
        {
            get { return GetTable<TEntity>().OfType<TEntity>().AsQueryable(); }
        }

        public void SetLoadOptions(DataLoadOptions loadOptions)
        {
            Context.LoadOptions = loadOptions;
        }

        public void ToggleDeferredLoading(bool deferredLoadingEnabled)
        {
            Context.DeferredLoadingEnabled = deferredLoadingEnabled;
        }

        void IRepository<TEntity>.Add(TEntity entity)
        {
            Add(entity);
        }

        void IRepository<TEntity>.Add(IEnumerable<TEntity> entities)
        {
            Add(entities);
        }

        public TDomain FindDomainObject<TDomain>(TDomain domainObject) where TDomain : class
        {
            var result = from dO in Find<TDomain>()
                         where dO == domainObject
                         select dO;
            return result.FirstOrDefault();
        }

        void IRepository<TEntity>.Delete(TEntity entity)
        {
            Delete(entity);
        }

        void IRepository<TEntity>.Persist()
        {
            Context.SubmitChanges();
        }

        void IRepository<TEntity>.MergePersist()
        {
            try
            {
                Context.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException ex)
            {
                //Resolves any conflicts that occur.
                //RefreshMode.KeepChanges = Resolve Concurrency Conflicts by Merging with Database Values (Merge)
                //RefreshMode.KeepCurrentValues = Resolve Concurrency Conflicts by Overwriting Database Values (Use yours)
                //RefreshMode.OverwriteCurrentValues = Resolve Concurrency Conflicts by Retaining Database Values (Use theirs)

                Log.Debug("A conflict has occured and is being resolved by IRepository<TEntity>.MergePersist()", ex);

                Context.ChangeConflicts.ResolveAll(RefreshMode.KeepChanges, false);
                Context.SubmitChanges(ConflictMode.FailOnFirstConflict);
            }
        }

        public void Add(TEntity entity)
        {
            GetTable<TEntity>().InsertOnSubmit(entity);
        }

        public void Add(IEnumerable<TEntity> entities)
        {
            GetTable<TEntity>().InsertAllOnSubmit(entities);
        }

        public void Delete(TEntity entity)
        {
            Delete<TEntity>(entity);
        }

        protected void Delete<T>(T entity) where T:class 
        {
            GetTable<T>().DeleteOnSubmit(entity);            
        }

        public void Delete(IEnumerable<TEntity> items)
        {
            GetTable<TEntity>().DeleteAllOnSubmit(items);
        }

        public void Persist()
        {
            Context.SubmitChanges();
        }

        IEnumerable<TEntity> IRepository<TEntity>.Find(Expression<Func<TEntity, bool>> criteria)
        {
            return RepositoryQuery.Where(criteria);
        }

        TEntity IRepository<TEntity>.FindOne(Expression<Func<TEntity, bool>> criteria)
        {
            IEnumerable<TEntity> foundItems = ((IRepository<TEntity>)this).Find(criteria);
            if (foundItems.Count() != 1)
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The given criteria returns {0} results, exactly one result is expected", foundItems.Count()), "criteria");
            return foundItems.First();
        }

        public T AttachItem<T>(T item) where T : class
        {
            var detachedItem = EntityDetacher<T>.Detach(item);
            GetTable<T>().Attach(detachedItem);

            return detachedItem;
        }
    }

    public class EntityDetacher<T>
    {
        //detach a LINQ entity from it's previous datacontext by serialization / deserialization
        static public T Detach(T item)
        {
            string detachedProp = Serialize(item);
            return (T)Deserialize(typeof(T), detachedProp);
        }

        static private string Serialize(object value)
        {
            if (value.GetType() == typeof(string))
                return value.ToString();

            var stringWriter = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(stringWriter))
            {
                var serializer = new
                    DataContractSerializer(value.GetType());
                serializer.WriteObject(writer, value);
            }

            return stringWriter.ToString();
        }

        static private object Deserialize(Type type, string serializedValue)
        {
            if (type == typeof(string))
                return serializedValue;

            using (var stringReader = new StringReader(serializedValue))
            {
                using (XmlReader reader = XmlReader.Create(stringReader))
                {
                    var serializer =
                        new DataContractSerializer((type));

                    object deserializedValue = serializer.ReadObject(reader);

                    return deserializedValue;
                }
            }
        }
    }
}