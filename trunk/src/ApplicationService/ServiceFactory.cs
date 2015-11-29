using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Consumentor.ShopGun.Component;

namespace Consumentor.ShopGun.ApplicationService
{
    public class ServiceFactory
    {
        private readonly IContainer _container;

        public ServiceFactory(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Wrap the returned object in a using statement for correct disposal of it.
        /// </summary>
        /// <returns></returns>
        public virtual TService Build<TService>()
        {
            return _container.Resolve<TService>();
        }
    }
}
