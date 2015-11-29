using System.Collections.Generic;
using System.Reflection;

namespace Consumentor.ShopGun.Repository
{
    public interface IAttributeMappingSource
    {
        IList<Assembly> MappingAssemblies { get; }
    }
}