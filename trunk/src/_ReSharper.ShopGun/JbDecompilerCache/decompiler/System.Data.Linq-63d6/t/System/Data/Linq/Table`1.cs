// Type: System.Data.Linq.Table`1
// Assembly: System.Data.Linq, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.5\System.Data.Linq.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Data.Linq.Provider;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace System.Data.Linq
{
  public sealed class Table<TEntity> : IQueryable<TEntity>, IQueryProvider, IEnumerable<TEntity>, ITable, IQueryable, IEnumerable, IListSource where TEntity : class
  {
    private DataContext context;
    private MetaTable metaTable;
    private IBindingList cachedList;

    public DataContext Context
    {
      get
      {
        return this.context;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return !this.metaTable.RowType.IsEntity;
      }
    }

    Expression IQueryable.Expression
    {
      get
      {
        return (Expression) Expression.Constant((object) this);
      }
    }

    Type IQueryable.ElementType
    {
      get
      {
        return typeof (TEntity);
      }
    }

    IQueryProvider IQueryable.Provider
    {
      get
      {
        return (IQueryProvider) this;
      }
    }

    bool IListSource.ContainsListCollection
    {
      get
      {
        return false;
      }
    }

    internal Table(DataContext context, MetaTable metaTable)
    {
      this.context = context;
      this.metaTable = metaTable;
    }

    IQueryable IQueryProvider.CreateQuery(Expression expression)
    {
      if (expression == null)
        throw System.Data.Linq.Error.ArgumentNull("expression");
      Type elementType = TypeSystem.GetElementType(expression.Type);
      Type type = typeof (IQueryable<>).MakeGenericType(new Type[1]
      {
        elementType
      });
      if (!type.IsAssignableFrom(expression.Type))
        throw System.Data.Linq.Error.ExpectedQueryableArgument((object) "expression", (object) type);
      return (IQueryable) Activator.CreateInstance(typeof (DataQuery<>).MakeGenericType(new Type[1]
      {
        elementType
      }), new object[2]
      {
        (object) this.context,
        (object) expression
      });
    }

    IQueryable<TResult> IQueryProvider.CreateQuery<TResult>(Expression expression)
    {
      if (expression == null)
        throw System.Data.Linq.Error.ArgumentNull("expression");
      if (!typeof (IQueryable<TResult>).IsAssignableFrom(expression.Type))
        throw System.Data.Linq.Error.ExpectedQueryableArgument((object) "expression", (object) typeof (IEnumerable<TResult>));
      else
        return (IQueryable<TResult>) new DataQuery<TResult>(this.context, expression);
    }

    object IQueryProvider.Execute(Expression expression)
    {
      return this.context.Provider.Execute(expression).ReturnValue;
    }

    TResult IQueryProvider.Execute<TResult>(Expression expression)
    {
      return (TResult) this.context.Provider.Execute(expression).ReturnValue;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public IEnumerator<TEntity> GetEnumerator()
    {
      return ((IEnumerable<TEntity>) this.context.Provider.Execute((Expression) Expression.Constant((object) this)).ReturnValue).GetEnumerator();
    }

    IList IListSource.GetList()
    {
      if (this.cachedList == null)
        this.cachedList = this.GetNewBindingList();
      return (IList) this.cachedList;
    }

    public IBindingList GetNewBindingList()
    {
      return BindingList.Create<TEntity>(this.context, (IEnumerable<TEntity>) this);
    }

    public void InsertOnSubmit(TEntity entity)
    {
      if ((object) entity == null)
        throw System.Data.Linq.Error.ArgumentNull("entity");
      this.CheckReadOnly();
      this.context.CheckNotInSubmitChanges();
      this.context.VerifyTrackingEnabled();
      MetaType inheritanceType = this.metaTable.RowType.GetInheritanceType(entity.GetType());
      if (!Table<TEntity>.IsTrackableType(inheritanceType))
        throw System.Data.Linq.Error.TypeCouldNotBeAdded((object) inheritanceType.Type);
      TrackedObject trackedObject = this.context.Services.ChangeTracker.GetTrackedObject((object) entity);
      if (trackedObject == null)
        this.context.Services.ChangeTracker.Track((object) entity).ConvertToNew();
      else if (trackedObject.IsWeaklyTracked)
        trackedObject.ConvertToNew();
      else if (trackedObject.IsDeleted)
        trackedObject.ConvertToPossiblyModified();
      else if (trackedObject.IsRemoved)
        trackedObject.ConvertToNew();
      else if (!trackedObject.IsNew)
        throw System.Data.Linq.Error.CantAddAlreadyExistingItem();
    }

    void ITable.InsertOnSubmit(object entity)
    {
      if (entity == null)
        throw System.Data.Linq.Error.ArgumentNull("entity");
      TEntity entity1 = entity as TEntity;
      if ((object) entity1 == null)
        throw System.Data.Linq.Error.EntityIsTheWrongType();
      this.InsertOnSubmit(entity1);
    }

    public void InsertAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities) where TSubEntity : TEntity
    {
      if (entities == null)
        throw System.Data.Linq.Error.ArgumentNull("entities");
      this.CheckReadOnly();
      this.context.CheckNotInSubmitChanges();
      this.context.VerifyTrackingEnabled();
      foreach (TSubEntity subEntity in System.Linq.Enumerable.ToList<TSubEntity>(entities))
        this.InsertOnSubmit((TEntity) subEntity);
    }

    void ITable.InsertAllOnSubmit(IEnumerable entities)
    {
      if (entities == null)
        throw System.Data.Linq.Error.ArgumentNull("entities");
      this.CheckReadOnly();
      this.context.CheckNotInSubmitChanges();
      this.context.VerifyTrackingEnabled();
      List<object> list = System.Linq.Enumerable.ToList<object>(System.Linq.Enumerable.Cast<object>(entities));
      ITable table = (ITable) this;
      foreach (object entity in list)
        table.InsertOnSubmit(entity);
    }

    private static bool IsTrackableType(MetaType type)
    {
      if (type == null || !type.CanInstantiate || type.HasInheritance && !type.HasInheritanceCode)
        return false;
      else
        return true;
    }

    public void DeleteOnSubmit(TEntity entity)
    {
      if ((object) entity == null)
        throw System.Data.Linq.Error.ArgumentNull("entity");
      this.CheckReadOnly();
      this.context.CheckNotInSubmitChanges();
      this.context.VerifyTrackingEnabled();
      TrackedObject trackedObject = this.context.Services.ChangeTracker.GetTrackedObject((object) entity);
      if (trackedObject == null)
        throw System.Data.Linq.Error.CannotRemoveUnattachedEntity();
      if (trackedObject.IsNew)
      {
        trackedObject.ConvertToRemoved();
      }
      else
      {
        if (!trackedObject.IsPossiblyModified && !trackedObject.IsModified)
          return;
        trackedObject.ConvertToDeleted();
      }
    }

    void ITable.DeleteOnSubmit(object entity)
    {
      if (entity == null)
        throw System.Data.Linq.Error.ArgumentNull("entity");
      TEntity entity1 = entity as TEntity;
      if ((object) entity1 == null)
        throw System.Data.Linq.Error.EntityIsTheWrongType();
      this.DeleteOnSubmit(entity1);
    }

    public void DeleteAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities) where TSubEntity : TEntity
    {
      if (entities == null)
        throw System.Data.Linq.Error.ArgumentNull("entities");
      this.CheckReadOnly();
      this.context.CheckNotInSubmitChanges();
      this.context.VerifyTrackingEnabled();
      foreach (TSubEntity subEntity in System.Linq.Enumerable.ToList<TSubEntity>(entities))
        this.DeleteOnSubmit((TEntity) subEntity);
    }

    void ITable.DeleteAllOnSubmit(IEnumerable entities)
    {
      if (entities == null)
        throw System.Data.Linq.Error.ArgumentNull("entities");
      this.CheckReadOnly();
      this.context.CheckNotInSubmitChanges();
      this.context.VerifyTrackingEnabled();
      List<object> list = System.Linq.Enumerable.ToList<object>(System.Linq.Enumerable.Cast<object>(entities));
      ITable table = (ITable) this;
      foreach (object entity in list)
        table.DeleteOnSubmit(entity);
    }

    public void Attach(TEntity entity)
    {
      if ((object) entity == null)
        throw System.Data.Linq.Error.ArgumentNull("entity");
      this.Attach(entity, false);
    }

    void ITable.Attach(object entity)
    {
      if (entity == null)
        throw System.Data.Linq.Error.ArgumentNull("entity");
      TEntity entity1 = entity as TEntity;
      if ((object) entity1 == null)
        throw System.Data.Linq.Error.EntityIsTheWrongType();
      this.Attach(entity1, false);
    }

    public void Attach(TEntity entity, bool asModified)
    {
      if ((object) entity == null)
        throw System.Data.Linq.Error.ArgumentNull("entity");
      this.CheckReadOnly();
      this.context.CheckNotInSubmitChanges();
      this.context.VerifyTrackingEnabled();
      MetaType inheritanceType = this.metaTable.RowType.GetInheritanceType(entity.GetType());
      if (!Table<TEntity>.IsTrackableType(inheritanceType))
        throw System.Data.Linq.Error.TypeCouldNotBeTracked((object) inheritanceType.Type);
      if (asModified && (inheritanceType.VersionMember == null && inheritanceType.HasUpdateCheck))
        throw System.Data.Linq.Error.CannotAttachAsModifiedWithoutOriginalState();
      TrackedObject trackedObject = this.Context.Services.ChangeTracker.GetTrackedObject((object) entity);
      if (trackedObject != null && !trackedObject.IsWeaklyTracked)
        throw System.Data.Linq.Error.CannotAttachAlreadyExistingEntity();
      if (trackedObject == null)
        trackedObject = this.context.Services.ChangeTracker.Track((object) entity, true);
      if (asModified)
        trackedObject.ConvertToModified();
      else
        trackedObject.ConvertToUnmodified();
      if (this.Context.Services.InsertLookupCachedObject(inheritanceType, (object) entity) != (object) entity)
        throw new DuplicateKeyException((object) entity, System.Data.Linq.Strings.CantAddAlreadyExistingKey);
      trackedObject.InitializeDeferredLoaders();
    }

    void ITable.Attach(object entity, bool asModified)
    {
      if (entity == null)
        throw System.Data.Linq.Error.ArgumentNull("entity");
      TEntity entity1 = entity as TEntity;
      if ((object) entity1 == null)
        throw System.Data.Linq.Error.EntityIsTheWrongType();
      this.Attach(entity1, asModified);
    }

    public void Attach(TEntity entity, TEntity original)
    {
      if ((object) entity == null)
        throw System.Data.Linq.Error.ArgumentNull("entity");
      if ((object) original == null)
        throw System.Data.Linq.Error.ArgumentNull("original");
      if (entity.GetType() != original.GetType())
        throw System.Data.Linq.Error.OriginalEntityIsWrongType();
      this.CheckReadOnly();
      this.context.CheckNotInSubmitChanges();
      this.context.VerifyTrackingEnabled();
      MetaType inheritanceType = this.metaTable.RowType.GetInheritanceType(entity.GetType());
      if (!Table<TEntity>.IsTrackableType(inheritanceType))
        throw System.Data.Linq.Error.TypeCouldNotBeTracked((object) inheritanceType.Type);
      TrackedObject trackedObject = this.context.Services.ChangeTracker.GetTrackedObject((object) entity);
      if (trackedObject != null && !trackedObject.IsWeaklyTracked)
        throw System.Data.Linq.Error.CannotAttachAlreadyExistingEntity();
      if (trackedObject == null)
        trackedObject = this.context.Services.ChangeTracker.Track((object) entity, true);
      trackedObject.ConvertToPossiblyModified((object) original);
      if (this.Context.Services.InsertLookupCachedObject(inheritanceType, (object) entity) != (object) entity)
        throw new DuplicateKeyException((object) entity, System.Data.Linq.Strings.CantAddAlreadyExistingKey);
      trackedObject.InitializeDeferredLoaders();
    }

    void ITable.Attach(object entity, object original)
    {
      if (entity == null)
        throw System.Data.Linq.Error.ArgumentNull("entity");
      if (original == null)
        throw System.Data.Linq.Error.ArgumentNull("original");
      this.CheckReadOnly();
      this.context.CheckNotInSubmitChanges();
      this.context.VerifyTrackingEnabled();
      TEntity entity1 = entity as TEntity;
      if ((object) entity1 == null)
        throw System.Data.Linq.Error.EntityIsTheWrongType();
      if (entity.GetType() != original.GetType())
        throw System.Data.Linq.Error.OriginalEntityIsWrongType();
      this.Attach(entity1, (TEntity) original);
    }

    public void AttachAll<TSubEntity>(IEnumerable<TSubEntity> entities) where TSubEntity : TEntity
    {
      if (entities == null)
        throw System.Data.Linq.Error.ArgumentNull("entities");
      this.AttachAll<TSubEntity>(entities, false);
    }

    void ITable.AttachAll(IEnumerable entities)
    {
      if (entities == null)
        throw System.Data.Linq.Error.ArgumentNull("entities");
      this.AttachAll(entities, false);
    }

    public void AttachAll<TSubEntity>(IEnumerable<TSubEntity> entities, bool asModified) where TSubEntity : TEntity
    {
      if (entities == null)
        throw System.Data.Linq.Error.ArgumentNull("entities");
      this.CheckReadOnly();
      this.context.CheckNotInSubmitChanges();
      this.context.VerifyTrackingEnabled();
      foreach (TSubEntity subEntity in System.Linq.Enumerable.ToList<TSubEntity>(entities))
        this.Attach((TEntity) subEntity, asModified);
    }

    void ITable.AttachAll(IEnumerable entities, bool asModified)
    {
      if (entities == null)
        throw System.Data.Linq.Error.ArgumentNull("entities");
      this.CheckReadOnly();
      this.context.CheckNotInSubmitChanges();
      this.context.VerifyTrackingEnabled();
      List<object> list = System.Linq.Enumerable.ToList<object>(System.Linq.Enumerable.Cast<object>(entities));
      ITable table = (ITable) this;
      foreach (object entity in list)
        table.Attach(entity, asModified);
    }

    public TEntity GetOriginalEntityState(TEntity entity)
    {
      if ((object) entity == null)
        throw System.Data.Linq.Error.ArgumentNull("entity");
      MetaType metaType = this.Context.Mapping.GetMetaType(entity.GetType());
      if (metaType == null || !metaType.IsEntity)
        throw System.Data.Linq.Error.EntityIsTheWrongType();
      TrackedObject trackedObject = this.Context.Services.ChangeTracker.GetTrackedObject((object) entity);
      if (trackedObject == null)
        return default (TEntity);
      if (trackedObject.Original != null)
        return (TEntity) trackedObject.CreateDataCopy(trackedObject.Original);
      else
        return (TEntity) trackedObject.CreateDataCopy(trackedObject.Current);
    }

    object ITable.GetOriginalEntityState(object entity)
    {
      if (entity == null)
        throw System.Data.Linq.Error.ArgumentNull("entity");
      TEntity entity1 = entity as TEntity;
      if ((object) entity1 == null)
        throw System.Data.Linq.Error.EntityIsTheWrongType();
      else
        return (object) this.GetOriginalEntityState(entity1);
    }

    public ModifiedMemberInfo[] GetModifiedMembers(TEntity entity)
    {
      if ((object) entity == null)
        throw System.Data.Linq.Error.ArgumentNull("entity");
      MetaType metaType = this.Context.Mapping.GetMetaType(entity.GetType());
      if (metaType == null || !metaType.IsEntity)
        throw System.Data.Linq.Error.EntityIsTheWrongType();
      TrackedObject trackedObject = this.Context.Services.ChangeTracker.GetTrackedObject((object) entity);
      if (trackedObject != null)
        return System.Linq.Enumerable.ToArray<ModifiedMemberInfo>(trackedObject.GetModifiedMembers());
      else
        return new ModifiedMemberInfo[0];
    }

    ModifiedMemberInfo[] ITable.GetModifiedMembers(object entity)
    {
      if (entity == null)
        throw System.Data.Linq.Error.ArgumentNull("entity");
      TEntity entity1 = entity as TEntity;
      if ((object) entity1 == null)
        throw System.Data.Linq.Error.EntityIsTheWrongType();
      else
        return this.GetModifiedMembers(entity1);
    }

    private void CheckReadOnly()
    {
      if (this.IsReadOnly)
        throw System.Data.Linq.Error.CannotPerformCUDOnReadOnlyTable((object) this.ToString());
    }

    public override string ToString()
    {
      return "Table(" + typeof (TEntity).Name + ")";
    }
  }
}
