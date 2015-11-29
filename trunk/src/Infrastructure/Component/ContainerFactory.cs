using Consumentor.ShopGun.Configuration;

namespace Consumentor.ShopGun.Component
{
    public class ContainerFactory
    {
        private readonly IContainerConfigurationSettings _configuration;

        public ContainerFactory(IContainerConfigurationSettings configuration)
        {
            _configuration = configuration;
        }

        public static IContainer CreateContainer()
        {
            return (new ContainerFactory(new ContainerConfiguration())).Create();
        }

        public IContainer Create()
        {
            var container = new Container(_configuration);
            RegisterComponents(container);
            return container;
        }

        public void RegisterComponents(IContainer container)
        {
            if (_configuration.UseWindsorCodeConfiguration)
            {
                var containerConfigurationSetup = container.Resolve<IContainerConfiguration>();
                containerConfigurationSetup.Setup();
            }
        }
    }
}