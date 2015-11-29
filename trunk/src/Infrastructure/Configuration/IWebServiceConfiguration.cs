namespace Consumentor.ShopGun.Configuration
{
    public interface IWebServiceConfiguration
    {
        string UserAgent { get; }
        string IMEI { get; }
        string Model { get; }
        string OsVersion { get; }
    }
}