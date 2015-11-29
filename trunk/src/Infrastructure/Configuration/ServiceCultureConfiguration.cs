using System.Globalization;

namespace Consumentor.ShopGun.Configuration
{
    public class ServiceCultureConfiguration : ConfigurationBase, IServiceCultureConfiguration
    {
        CultureInfo IServiceCultureConfiguration.CultureInfo
        {
            get
            {
                return GetCulture("CultureInfo", CultureInfo.CurrentCulture);
            }
        }

        CultureInfo IServiceCultureConfiguration.UICulture
        {
            get
            {
                return GetCulture("UICulture", CultureInfo.CurrentUICulture);
            }
        }
        private CultureInfo GetCulture(string configKey, CultureInfo defaultValue)
        {
            string culture = base.GetValueFromAppConfig(configKey, defaultValue.ToString());
            return new CultureInfo(culture);
        }
    }
}
