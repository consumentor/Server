using System;
using System.Linq;
using System.Text;

namespace Consumentor.ShopGun.Services
{
    public static class ServiceName
    {
        public static string GetName(Type serviceType)
        {
            string[] nameSpaceParts = serviceType.FullName.Split(new[] { '.' });
            nameSpaceParts = (from ns in nameSpaceParts
                             where ns != "ApplicationService"
                             select ns).ToArray();
            var serviceName = new StringBuilder();
            int max = nameSpaceParts.Length > 4 ? 4 : nameSpaceParts.Length;
            for (int i = 0; i < max; i++)
            {
                serviceName.Append(nameSpaceParts[i]);
                if (i + 1 < max)
                    serviceName.Append(".");
            }
            serviceName.Append(" v");
            serviceName.Append(serviceType.Assembly.GetName().Version);
            return serviceName.ToString();
        }
    }
}