using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server.WebService
{
    [ServiceContract]
    public interface IMentorContract
    {
        [OperationContract]
        //[WebGet(UriTemplate = "Search/{queryString}", ResponseFormat = WebMessageFormat.Xml)]
        [WebGet(UriTemplate = "/xml/GetMentors", ResponseFormat = WebMessageFormat.Xml)]
        List<Mentor> GetMentorsXml();

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            UriTemplate = "/GetMentors",       
            ResponseFormat = WebMessageFormat.Json)]
        List<Mentor> GetMentors();

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            UriTemplate = "/GetProductsForMentor",
            ResponseFormat = WebMessageFormat.Json)]
        List<Product> GetProductsForMentor(int mentorId);
    }
}
