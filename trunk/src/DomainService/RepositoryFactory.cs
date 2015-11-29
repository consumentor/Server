using System;
using Consumentor.ShopGun.Component;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.DomainService
{
    public class RepositoryFactory
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
        public virtual TRepository Build<TRepository, TDomain>() 
            where TRepository : IDisposable, IRepository<TDomain>
            where TDomain : class
        {
            return _container.Resolve<TRepository>();
        }
    }
}
