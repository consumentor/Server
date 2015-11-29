using Consumentor.ShopGun.Configuration;

namespace Consumentor.ShopGun.Gateway.Configuration
{
    public class OPVWebServiceConfiguration : ConfigurationBase, IOPVWebServiceConfiguration
    {
        private const string WebServiceUsername = "WebService_Username";
        private const string WebServicePassword = "WebService_Password";
        private const string PrimaryWebServiceAddress = "PrimaryWebServiceAddress";
        private const string SecondaryWebServiceAddress = "SecondaryWebServiceAddress";


        public string Username
        {
            get { return GetValueFromAppConfig(WebServiceUsername, ""); }
        }

        public string Password
        {
            get { return GetValueFromAppConfig(WebServicePassword, ""); }
        }

        public string PrimaryUrl
        {
            get { return GetValueFromAppConfig(PrimaryWebServiceAddress, ""); }
        }

        public string SecondaryUrl
        {
            get { return GetValueFromAppConfig(SecondaryWebServiceAddress, ""); }
        }

    }
}