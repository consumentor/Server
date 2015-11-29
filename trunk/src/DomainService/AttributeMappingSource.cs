using System.Collections.Generic;
using System.Reflection;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.DomainService
{
    public class AttributeMappingSource : IAttributeMappingSource
    {
        #region IAttributeMappingSource Members

        IList<Assembly> IAttributeMappingSource.MappingAssemblies
        {
            get { return new List<Assembly> { typeof(Base).Assembly }; }
        }

        #endregion
    }
}