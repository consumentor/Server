namespace Consumentor.ShopGun.Configuration
{
    public class ContainerConfiguration : BasicConfiguration, IContainerConfigurationSettings
    {
        string IContainerConfigurationSettings.ComponentConfigurationFileName
        {
            get { return GetValueFromAppConfig("ComponentConfigurationFileName", "Windsor.Config"); }
        }

        bool IContainerConfigurationSettings.UseWindsorCodeConfiguration
        {
            get
            {
                return bool.Parse(GetValueFromAppConfig("UseWindsorCodeConfiguration", "true"));
            }
        }
    }
}