using Consumentor.ShopGun.Context;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    public interface IAdviceSearchApplicationService
    {
        SearchResultMessageEntity Search(string userSubcriptionToken, string queryString, IShopgunWebOperationContext shopgunWebOperationContext);

        ItemInfoMessageEntity SearchItems(string userSubscriptionToken, string queryString, int[] maxHitsPerCategory,
                                          IShopgunWebOperationContext shopgunWebOperationContext);
        ItemInfoMessageEntity SearchListInfo(string userSubscriptionToken, string queryString, int maxNumHitsPerCategory, IShopgunWebOperationContext shopgunWebOperationContext);
    }
}