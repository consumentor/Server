using System.ServiceModel;
using System.ServiceModel.Web;
using Consumentor.ShopGun.Domain;


namespace Consumentor.ShopGun.ApplicationService.Server.WebService
{
    [ServiceContract(Namespace = Base.DataContractNamespace)]
    public interface IAdviceSearchContract
    {
        [OperationContract]
        //[WebGet(UriTemplate = "Search/{queryString}", ResponseFormat = WebMessageFormat.Xml)]
        [WebGet(UriTemplate = "/subscription/{userSubcriptionToken}/search/?q={queryString}", ResponseFormat = WebMessageFormat.Xml)]
        SearchResultMessageEntity Search(string userSubcriptionToken, string queryString);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "search/{userSubcriptionToken}/{queryString}", ResponseFormat = WebMessageFormat.Json)]
        SearchResultMessageEntity SearchJson(string userSubcriptionToken, string queryString);

        [OperationContract]
        //[WebGet(UriTemplate = "Search/{queryString}", ResponseFormat = WebMessageFormat.Xml)]
        [WebGet(UriTemplate = "/subscription/{userSubcriptionToken}/searchListInfo/?q={queryString}", ResponseFormat = WebMessageFormat.Xml)]
        ItemInfoMessageEntity SearchListInfo(string userSubcriptionToken, string queryString);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "searchListInfo/{userSubcriptionToken}/{queryString}", ResponseFormat = WebMessageFormat.Json)]
        ItemInfoMessageEntity SearchListInfoJson(string userSubcriptionToken, string queryString);
    }
}
