using System.ServiceModel;
using System.ServiceModel.Web;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Gateway.Server;


namespace Consumentor.ShopGun.ApplicationService.Server.WebService
{
    [ServiceContract(Namespace = Base.DataContractNamespace)]
    public interface IShopgunMembershipContract
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "ValidateUser/{username}/{password}", ResponseFormat = WebMessageFormat.Json)]
        ShopgunMembershipWebserviceGateway ValidateMobileUser(string username, string password);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "CreateUser/{username}/{password}/{email}", ResponseFormat = WebMessageFormat.Json)]
        ShopgunMembershipWebserviceGateway CreateUser(string username, string password, string email);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Users", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ShopgunMembershipWebserviceGateway CreateNewUser(string username, string password, string email);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "tokens/google", ResponseFormat = WebMessageFormat.Json)]
        ShopgunMembershipWebserviceGateway ValidateGoogleUser();
    }
}