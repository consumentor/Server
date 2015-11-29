using System.Globalization;

namespace Consumentor.ShopGun.Configuration
{
    public interface IServiceCultureConfiguration
    {
        CultureInfo CultureInfo { get; }
        CultureInfo UICulture { get; }
    }
}