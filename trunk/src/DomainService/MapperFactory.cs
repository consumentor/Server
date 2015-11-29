using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Consumentor.ShopGun.Component;
using Consumentor.ShopGun.Gateway;

namespace Consumentor.ShopGun.DomainService
{
    public class MapperFactory
    {

        private readonly IContainer _container;
        public MapperFactory(IContainer container)
        {
            _container = container;
        }

        public T Build<T>() where T : class
        {
            return _container.Resolve<T>();
        }
    }
}
