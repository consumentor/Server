using Consumentor.ShopGun.Component;

namespace Consumentor.ShopGun.Configuration
{
    public interface IConfiguration
    {
        string ConnectionString { get; }
        IContainer Container { get; }
    }
}