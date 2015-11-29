using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace ShopGunSpecBase
{
    public class ServiceUnderTest
    {
        private string _executable;
        public string Executable
        {
            get { return _executable; }
            set { _executable = Path.GetFullPath(value); }
        }

        public string Arguments { get; set; }

        private readonly Dictionary<string, string> _originalConnectionStringsConfigFile = new Dictionary<string, string>();

        public void ModifyAppConfigConnectionString(string name, string connectionString)
        {
            var fileName = Path.Combine(Path.GetDirectoryName(Executable), "ConnectionStrings.config");
            var appConfig = new XmlDocument();
            appConfig.Load(fileName);
            if (_originalConnectionStringsConfigFile.ContainsKey(fileName) == false)
                _originalConnectionStringsConfigFile.Add(fileName, appConfig.OuterXml);
            var node = appConfig.SelectSingleNode(string.Format(CultureInfo.CurrentCulture, "/connectionStrings/add[@name='{0}']", name));
            node.Attributes["connectionString"].Value = connectionString;
            appConfig.Save(fileName);
        }

        public void ModifyAppConfigAppSettings(string key, string value)
        {
            var fileName = Executable + ".config";
            var appConfig = new XmlDocument();
            appConfig.Load(fileName);
            var node = appConfig.SelectSingleNode(string.Format(CultureInfo.CurrentCulture, "/configuration/appSettings/add[@key='{0}']", key));
            if (node != null)
            {
                node.Attributes["value"].Value = value;
            }
            else
            {
                XmlElement elem = appConfig.CreateElement("add");
                elem.SetAttribute("key", key);
                elem.SetAttribute("value", value);
                appConfig.SelectSingleNode("//appSettings").AppendChild(elem);
            }
            appConfig.Save(fileName);
        }

        public void ModifyAppConfigAppSettings(IDictionary<string, string> appConfigValues)
        {
            if (appConfigValues != null)
            {
                foreach (KeyValuePair<string, string> appConfigValue in appConfigValues)
                    ModifyAppConfigAppSettings(appConfigValue.Key, appConfigValue.Value);
            }
        }

        public void RestoreConnectionStringsConfigFiles()
        {
            foreach (var fileName in _originalConnectionStringsConfigFile.Keys)
            {
                File.WriteAllText(fileName, _originalConnectionStringsConfigFile[fileName]);
            }
        }
    }
}
