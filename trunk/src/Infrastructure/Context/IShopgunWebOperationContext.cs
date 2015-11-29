namespace Consumentor.ShopGun.Context
{
    public interface IShopgunWebOperationContext
    {
        string UserAgent { get; }
        string IMEI { get; }
        string Model { get; }
        string OsVersion { get;}
    }
}
