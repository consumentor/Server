namespace Consumentor.ShopGun.Gateway.Configuration
{
    public interface IOPVWebServiceConfiguration
    {
        string Username { get; }
        string Password { get; }
        string PrimaryUrl { get; }
        string SecondaryUrl { get; }
    }
}