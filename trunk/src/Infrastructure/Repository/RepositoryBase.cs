using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace Consumentor.ShopGun.Repository
{
    public abstract class RepositoryBase : IDisposable
    {
        private DataContext _context;
        private bool _isDisposed;

        protected RepositoryBase(DataContext context)
        {
            CheckContextNull(context);
            Context = context;
        }

        protected DataContext Context
        {
            get { return _context; }
            set
            {
                CheckContextNull(value);
                _context = value;
            }
        }

        private void CheckContextNull(DataContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context", "cannot be null");
        }

        protected ITable GetTable<T>() where T : class
        {
            return Context.GetTableForType(typeof(T));
        }

        protected IQueryable<T> Find<T>() where T : class
        {
            return GetTable<T>().OfType<T>().AsQueryable();
        }

        protected void DeleteAll<T>(IList<T> entities) where T : class
        {
            GetTable<T>().DeleteAllOnSubmit(entities);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed == false)
            {
                if (disposing)
                {
                    if (_context != null)
                        _context.Dispose();
                }
            }
            _isDisposed = true;
        }

    }
}