using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Domain.DataTransferObject;
using ProductInfo = Consumentor.ShopGun.Domain.DataTransferObject.ProductInfo;

namespace Consumentor.ShopGun.ApplicationService.Server.WebService
{
    [ServiceContract(Namespace = Base.DataContractNamespace)]
    public interface IShopgunAppContract
    {
        [OperationContract]
        [WebGet(UriTemplate = "/xml/iteminfo/{query}?numResults={numResults}", ResponseFormat = WebMessageFormat.Xml)]
        ItemInfoMessageEntity GetItemInfo(string query, string numResults);

        [OperationContract]
        [WebGet(UriTemplate = "/iteminfo/{query}?numResults={maxResults}", ResponseFormat = WebMessageFormat.Json)]
        ItemInfoMessageEntity GetItemInfoJson(string query, string maxResults);

        [OperationContract]
        [WebGet(UriTemplate = "/xml/certificationmark/{certificationMarkId}", ResponseFormat = WebMessageFormat.Xml)]
        CertificationMark GetCertificationMark(string certificationMarkId);

        [OperationContract]
        [WebGet(UriTemplate = "/certificationmark/{certificationMarkId}", ResponseFormat = WebMessageFormat.Json)]
        CertificationMark GetCertificationMarkJson(string certificationMarkId);

        [OperationContract]
        [WebGet(UriTemplate = "/xml/certificationmarks", ResponseFormat = WebMessageFormat.Xml)]
        IList<CertificationMark> GetCertificationMarks();

        [OperationContract]
        [WebGet(UriTemplate = "/certificationmarks", ResponseFormat = WebMessageFormat.Json)]
        IList<CertificationMark> GetCertificationMarksJson();

        [OperationContract]
        [WebGet(UriTemplate = "/xml/certificationmarks/{certificationMarkId}", ResponseFormat = WebMessageFormat.Xml)]
        CertificationMark GetCertificationMark2(string certificationMarkId);

        [OperationContract]
        [WebGet(UriTemplate = "/certificationmarks/{certificationMarkId}", ResponseFormat = WebMessageFormat.Json)]
        CertificationMark GetCertificationMarkJson2(string certificationMarkId);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{userSubcriptionToken}/certificationmark/{certificationMarkId}",
            ResponseFormat = WebMessageFormat.Json)]
        CertificationMark GetCertificationMarkJsonPost(string userSubcriptionToken, string certificationMarkId);

        [OperationContract]
        [WebGet(UriTemplate = "XML/Mentor/{mentorId}", ResponseFormat = WebMessageFormat.Xml)]
        Mentor GetMentor(string mentorId);

        [OperationContract]
        [WebGet(UriTemplate = "Mentor/{mentorId}", ResponseFormat = WebMessageFormat.Json)]
        Mentor GetMentorJson(string mentorId);

        [OperationContract]
        [WebGet(UriTemplate = "XML/advisors/{mentorId}", ResponseFormat = WebMessageFormat.Xml)]
        Mentor GetAdvisor(string mentorId);

        [OperationContract]
        [WebGet(UriTemplate = "advisors/{mentorId}", ResponseFormat = WebMessageFormat.Json)]
        Mentor GetAdvisorJson(string mentorId);

        [OperationContract]
        [WebGet(UriTemplate = "XML/advisors", ResponseFormat = WebMessageFormat.Xml)]
        IList<Mentor> GetAdvisors();

        [OperationContract]
        [WebGet(UriTemplate = "Advisors", ResponseFormat = WebMessageFormat.Json)]
        IList<Mentor> GetAdvisorsJson();

        // For backward compatibility
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{userSubcriptionToken}/mentor/{mentorId}",
            ResponseFormat = WebMessageFormat.Json)]
        Mentor GetMentorJsonPost(string userSubcriptionToken, string mentorId);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "XML/Ingredients/{ingredientId}", ResponseFormat = WebMessageFormat.Xml
            )]
        Ingredient GetIngredient(string ingredientId);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/Ingredients/{ingredientId}", ResponseFormat = WebMessageFormat.Json)]
        Ingredient GetIngredientJson(string ingredientId);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "XML/Products" +
                                                 "?resultsPerPage={resultsPerPage}" +
                                                 "&pageIndex={pageIndex}" +
                                                 "&productCategoryId={categoryId}" +
                                                 "&hasCertificationMarks={hasCertificationMarks}" +
                                                 "&brandId={brandId}", ResponseFormat = WebMessageFormat.Xml)]
        IList<Product> GetProducts(string resultsPerPage, string pageIndex, string categoryId,
                                   string hasCertificationMarks, string brandId);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/Products" +
                                                 "?resultsPerPage={resultsPerPage}" +
                                                 "&pageIndex={pageIndex}" +
                                                 "&productCategoryId={categoryId}" +
                                                 "&hasCertificationMarks={hasCertificationMarks}" +
                                                 "&brandId={brandId}", ResponseFormat = WebMessageFormat.Json)]
        IList<Product> GetProductsJson(string resultsPerPage, string pageIndex, string categoryId,
                                       string hasCertificationMarks, string brandId);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "XML/Products/{productId}", ResponseFormat = WebMessageFormat.Xml)]
        Product GetProduct(string productId);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/Products/{productId}", ResponseFormat = WebMessageFormat.Json)]
        Product GetProductJson(string productId);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "XML/ProductsByGTIN/{gtin}", ResponseFormat = WebMessageFormat.Xml)]
        Product GetProductByGtin(string gtin);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/ProductsByGTIN/{gtin}", ResponseFormat = WebMessageFormat.Json)]
        Product GetProductGtinJson(string gtin);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "XML/ProductInfosByGTIN/{gtin}", ResponseFormat = WebMessageFormat.Xml)
        ]
        ProductInfo GetProductInfoByGtin(string gtin);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/ProductInfosByGTIN/{gtin}", ResponseFormat = WebMessageFormat.Json)]
        ProductInfo GetProductInfoGtinJson(string gtin);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "XML/Brands?resultsPerPage={resultsPerPage}&pageIndex={pageIndex}",
            ResponseFormat = WebMessageFormat.Xml)]
        IList<Brand> GetBrands(string resultsPerPage, string pageIndex);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/Brands?resultsPerPage={resultsPerPage}&pageIndex={pageIndex}",
            ResponseFormat = WebMessageFormat.Xml)]
        IList<Brand> GetBrandsJson(string resultsPerPage, string pageIndex);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "XML/Brands/{brandId}", ResponseFormat = WebMessageFormat.Xml)]
        Brand GetBrand(string brandId);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/Brands/{brandId}", ResponseFormat = WebMessageFormat.Json)]
        Brand GetBrandJson(string brandId);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "XML/Companies?isMember={isMember}&resultsPerPage={resultsPerPage}&pageIndex={pageIndex}",
            ResponseFormat = WebMessageFormat.Xml)]
        IList<Company> GetCompanies(string isMember, string resultsPerPage, string pageIndex);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/Companies?isMember={isMember}&resultsPerPage={resultsPerPage}&pageIndex={pageIndex}",
            ResponseFormat = WebMessageFormat.Json)]
        IList<Company> GetCompaniesJson(string isMember, string resultsPerPage, string pageIndex);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "XML/Companies/{companyId}", ResponseFormat = WebMessageFormat.Xml)]
        Company GetCompany(string companyId);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/Companies/{companyId}", ResponseFormat = WebMessageFormat.Json)]
        Company GetCompanyJson(string companyId);

        [OperationContract(Name = "GetCompanyBrands")]
        [WebInvoke(Method = "GET",
            UriTemplate = "XML/Companies/{companyId}/Brands?resultsPerPage={resultsPerPage}&pageIndex={pageIndex}",
            ResponseFormat = WebMessageFormat.Xml)]
        IList<Brand> GetBrands(string companyId, string resultsPerPage, string pageIndex);

        [OperationContract(Name = "GetCompanyBrandsJson")]
        [WebInvoke(Method = "GET",
            UriTemplate = "/Companies/{companyId}/Brands?resultsPerPage={resultsPerPage}&pageIndex={pageIndex}",
            ResponseFormat = WebMessageFormat.Json)]
        IList<Brand> GetBrandsJson(string companyId, string resultsPerPage, string pageIndex);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "XML/CompanyDetails/{companyId}", ResponseFormat = WebMessageFormat.Xml
            )]
        CompanyDetails GetCompanyDetails(string companyId);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/CompanyDetails/{companyId}", ResponseFormat = WebMessageFormat.Json)]
        CompanyDetails GetCompanyDetailsJson(string companyId);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "XML/Tips/Random", ResponseFormat = WebMessageFormat.Xml)]
        Tip GetRandomTip();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/Tips/Random", ResponseFormat = WebMessageFormat.Json)]
        Tip GetRandomTipJson();

        [OperationContract]
        [ServiceKnownType(typeof (IngredientAdvice))]
        [ServiceKnownType(typeof (ProductAdvice))]
        [ServiceKnownType(typeof (BrandAdvice))]
        [ServiceKnownType(typeof (CompanyAdvice))]
        [ServiceKnownType(typeof (CountryAdvice))]
        [ServiceKnownType(typeof (ConceptAdvice))]
        [WebInvoke(Method = "GET", UriTemplate = "XML/Advices/{adviceId}", ResponseFormat = WebMessageFormat.Xml)]
        AdviceBase GetAdvice(string adviceId);

        [OperationContract]
        [ServiceKnownType(typeof (IngredientAdvice))]
        [ServiceKnownType(typeof (ProductAdvice))]
        [ServiceKnownType(typeof (BrandAdvice))]
        [ServiceKnownType(typeof (CompanyAdvice))]
        [ServiceKnownType(typeof (CountryAdvice))]
        [ServiceKnownType(typeof (ConceptAdvice))]
        [WebInvoke(Method = "GET", UriTemplate = "/Advices/{adviceId}", ResponseFormat = WebMessageFormat.Json)]
        AdviceBase GetAdviceJson(string adviceId);

        [WebInvoke(Method = "GET", UriTemplate = "XML/Advices/{adviceId}/UserAdviceRatings",
            ResponseFormat = WebMessageFormat.Xml)]
        IList<UserAdviceRating> GetUserAdviceRatings(string adviceId);

        [WebInvoke(Method = "GET", UriTemplate = "Advices/{adviceId}/UserAdviceRatings",
            ResponseFormat = WebMessageFormat.Json)]
        IList<UserAdviceRating> GetUserAdviceRatingsJson(string adviceId);

        [WebInvoke(Method = "PUT", UriTemplate = "UserAdviceRatings",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        UserAdviceRating AddUserAdviceRating(UserAdviceRating userAdviceRating);

        [WebInvoke(Method = "GET", UriTemplate = "get-rating-for-advice?adviceId={adviceId}&deviceId={deviceId}",
            ResponseFormat = WebMessageFormat.Json)]
        int GetAdviceRatingJson(string adviceId, string deviceId);

        [WebInvoke(Method = "GET",
            UriTemplate = "/set-rating-for-advice?adviceId={adviceId}&deviceId={deviceId}&rating={rating}",
            ResponseFormat = WebMessageFormat.Json)]
        void SetAdviceRating(string adviceId, string deviceId, string rating);
    }
}