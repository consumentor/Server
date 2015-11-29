using System;

namespace Consumentor.ShopGun.Services
{
    public static class ServiceHostExtension
    {
        public static string WindowsServiceName(this Type type)
        {
            string serviceName = type.FullName;
            if (serviceName.Length > 80) //Apparently theres a restriction on the length of the name
            {
                serviceName = serviceName.Replace("Consumentor.Shopgun.", string.Empty);
            }
            if (serviceName.Length > 80) //If still too long, just use name
            {
                serviceName = type.Name;
            }
            return serviceName;
        }
    }
}