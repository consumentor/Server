using System;
using System.Configuration;
using System.Globalization;
using Castle.Core.Logging;

namespace Consumentor.ShopGun.Configuration
{
    public abstract class ConfigurationBase
    {
        private ILogger _log;

        public ILogger Log
        {
            get
            {
                if (_log == null) _log = NullLogger.Instance;
                return _log;
            }
            set { _log = value; }
        }

        protected virtual T GetValueFromAppConfig<T>(string keyName, T defaultValue)
        {
            return GetValue(ConfigurationManager.AppSettings[keyName], keyName, defaultValue);
        }

        private T GetValue<T>(object value, string keyName, T defaultValue)
        {
            //Log.Debug("Reading key {0} with defaultValue: {1}", keyName, defaultValue);
            if (value == null)
            {
                Log.Warn("Config key {0} is missing, returning default value: {1}", keyName, defaultValue);
                return defaultValue;
            }
            //Log.Debug("Config key {0} has value: {1}", keyName, value);
            if (typeof(T) == typeof(TimeSpan)) //Cant use Convert.ChangeType from string to TimeSpan :(
                value = TimeSpan.Parse(value.ToString());
            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }

        protected static void ProtectSection(string sectionName, string providerName)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationSection section = config.GetSection(sectionName);

            if (section == null)
                throw new ArgumentException("ProtectSection failed. The configuration section " + sectionName + " was not found.", "sectionName");

            if (!section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection(providerName);
                section.SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(sectionName);
            }
        }
    }
}