namespace Consumentor.ShopGun.Configuration
{
    public class WebServiceConfiguration : ConfigurationBase, IWebServiceConfiguration
    {
        private const string AppConfigHttpHeaderUserAgent = "AppConfigHttpHeaderUserAgent";
        private const string AppConfigHttpHeaderIMEI = "AppConfigHttpHeaderIMEI";
        private const string AppConfigHttpHeaderModel = "AppConfigHttpHeaderModel";
        private const string AppConfigHttpHeaderOsVersion = "AppConfigHttpHeaderOsVersion";


        public string UserAgent
        {
            get { return GetValueFromAppConfig(AppConfigHttpHeaderUserAgent, "User-Agent"); }
        }

        public string IMEI
        {
            get { return GetValueFromAppConfig(AppConfigHttpHeaderIMEI, "IMEI"); }
        }

        public string Model
        {
            get { return GetValueFromAppConfig(AppConfigHttpHeaderModel, "Model"); }
        }

        public string OsVersion
        {
            get { return GetValueFromAppConfig(AppConfigHttpHeaderOsVersion, "OS_Version"); }
        }
    }
}