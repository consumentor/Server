using System;
using Consumentor.ShopGun.Component;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.ApplicationService
{
    public class RepositoryFactory<T, TDomainObject>
        where T : class, IDisposable, IRepository<TDomainObject>
        where TDomainObject : class 
    {
        private readonly IContainer _container;

        protected RepositoryFactory()
        {
        }

        public RepositoryFactory(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Wrap the returned object in a using statement for correct disposal of it.
        /// </summary>
        /// <returns></returns>
        public virtual T Build()
        {
            return _container.Resolve<T>();
        }

        /// <summary>
        /// Wrap the returned object in a using statement for correct disposal of it.
        /// </summary>
        /// <returns></returns>
        public virtual TRepository Build<TRepository, TDomain>()
            where TRepository : IDisposable, IRepository<TDomain>
            where TDomain : class
        {
            return _container.Resolve<TRepository>();
        }
    }
}
