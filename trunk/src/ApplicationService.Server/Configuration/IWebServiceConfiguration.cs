namespace Consumentor.ShopGun.ApplicationService.Server.Configuration
{
    public interface IWebServiceConfiguration
    {
        string UserAgent { get; }
        string IMEI { get; }
        string Model { get; }
    }
}