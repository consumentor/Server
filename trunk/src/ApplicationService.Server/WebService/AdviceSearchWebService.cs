using System;
using System.ServiceModel;
using Consumentor.ShopGun.ApplicationService.WebService;
using Consumentor.ShopGun.Context;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server.WebService
{
    [ServiceBehavior(Namespace = Base.DataContractNamespace)]
    public class AdviceSearchWebService : WebServiceBase, IAdviceSearchContract
    {
        private readonly IAdviceSearchApplicationService _adviceSearchApplicationService;
        private readonly IShopgunWebOperationContext _shopgunWebOperationContext;

        public AdviceSearchWebService()
        {
            _adviceSearchApplicationService = Container.Resolve<IAdviceSearchApplicationService>();
            _shopgunWebOperationContext = Container.Resolve<IShopgunWebOperationContext>();
            Container.Resolve<IExternalProductInformationProviderApplicationService>();
        }

        public SearchResultMessageEntity Search(string userSubcriptionToken, string queryString)
        {
            try
            {
                //TODO: Validate user subscription token.
                return _adviceSearchApplicationService.Search(userSubcriptionToken, queryString,
                                                              _shopgunWebOperationContext);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
                throw;
            }
        }

        public SearchResultMessageEntity SearchJson(string userSubcriptionToken, string queryString)
        {
            try
            {
                //TODO: Validate user subscription token.
                return _adviceSearchApplicationService.Search(userSubcriptionToken, queryString,
                                                              _shopgunWebOperationContext);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
                throw;
            }
        }

        public ItemInfoMessageEntity SearchListInfo(string userSubcriptionToken, string queryString)
        {
            return _adviceSearchApplicationService.SearchListInfo(userSubcriptionToken, queryString, 5,
                                                                  _shopgunWebOperationContext);
        }

        public ItemInfoMessageEntity SearchListInfoJson(string userSubcriptionToken, string queryString)
        {
            return _adviceSearchApplicationService.SearchListInfo(userSubcriptionToken, queryString, 5,
                                                                  _shopgunWebOperationContext);
        }
    }
}
