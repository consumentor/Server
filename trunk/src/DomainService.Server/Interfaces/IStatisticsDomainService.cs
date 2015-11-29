using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.DomainService.Server.Interfaces
{
    public interface IStatisticsDomainService
    {
        void AddStatistics(User user, SearchResultMessageEntity messageEntity, string userAgent, string imei, string model, string osVersion);

        void AddAdviceRequest(User user, AdviceBase advice, string userAgent, string imei, string model, string osVersion);

        void AddIngredienRequest(int? userId, int ingredientId, string userAgent, string imei, string model, string osVersion);
        void AddProductRequest(int? userId, int productId, string userAgent, string imei, string model, string osVersion);
        void AddBrandRequest(int? userId, int brandId, string userAgent, string imei, string model, string osVersion);
        void AddCompanyRequest(int? userId, int companyId, string userAgent, string imei, string model, string osVersion);
        void AddCountryRequest(int? userId, int countryId, string userAgent, string imei, string model, string osVersion);
        void AddConceptRequest(int? userId, int conceptId, string userAgent, string imei, string model, string osVersion);

        void AddAdviceRequest(int? userId, int adviceId, string userAgent, string imei, string model, string osVersion);
    }
}