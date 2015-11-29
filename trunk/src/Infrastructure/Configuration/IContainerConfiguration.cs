namespace Consumentor.ShopGun.Configuration
{
    public interface IContainerConfigurationSettings
    {
        string ComponentConfigurationFileName { get; }
        bool UseWindsorCodeConfiguration { get; }
    }
}