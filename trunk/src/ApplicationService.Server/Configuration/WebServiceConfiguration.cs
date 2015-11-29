using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Consumentor.ShopGun.Configuration;

namespace Consumentor.ShopGun.ApplicationService.Server.Configuration
{
    public class WebServiceConfiguration : ConfigurationBase, IWebServiceConfiguration
    {
        private const string AppConfigHttpHeaderUserAgent = "User-Agent";
        private const string AppConfigHttpHeaderIMEI = "IMEI";
        private const string AppConfigHttpHeaderModel = "Model";


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
    }
}
