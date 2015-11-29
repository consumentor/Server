using System.Configuration;
using Consumentor.ShopGun.Component;

namespace Consumentor.ShopGun.Configuration
{
    public class BasicConfiguration : ConfigurationBase, IConfiguration
    {
        string IConfiguration.ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["ShopGun"].ConnectionString; }
        }

        private static IContainer _container;
        private static readonly object _lockObj = new object();
        private static bool _containerInitialized;
        IContainer IConfiguration.Container
        {
            get
            {
                if (_containerInitialized)
                    return _container;

                lock (_lockObj)
                {
                    if (_container == null)
                    {
                        _container = ContainerFactory.CreateContainer();
                        _containerInitialized = true;
                    }
                }
                return _container;
            }
        }
    }
}