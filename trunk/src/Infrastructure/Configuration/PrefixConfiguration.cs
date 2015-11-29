using System.Globalization;

namespace Consumentor.ShopGun.Configuration
{
    public abstract class PrefixConfiguration : ConfigurationBase
    {
        protected string ConfigKeyPrefix { get; set; }

        protected PrefixConfiguration(string configKeyPrefix)
        {
            ConfigKeyPrefix = configKeyPrefix;
        }

        protected virtual string GetFullKeyName(string prefix, string keyName)
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}.{1}", prefix, keyName);
        }

        protected override T GetValueFromAppConfig<T>(string keyName, T defaultValue)
        {
            return base.GetValueFromAppConfig(GetFullKeyName(ConfigKeyPrefix, keyName), defaultValue);
        }
    }
}