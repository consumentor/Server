using System;
using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server.Interfaces
{
    public interface IAdviceApplicationService
    {
        #region CRUD

        IngredientAdvice AddIngredientAdvice(Mentor mentor, IngredientAdvice advice);
        BrandAdvice AddBrandAdvice(Mentor mentor, BrandAdvice adviceToAdd);
        CompanyAdvice AddCompanyAdvice(Mentor mentor, CompanyAdvice adviceToAdd);
        CountryAdvice AddCountryAdvice(Mentor mentor, CountryAdvice adviceToAdd);
        ConceptAdvice AddConceptAdvice(Mentor mentor, ConceptAdvice adviceToAdd);

        [Obsolete]
        BrandAdvice AddBrandAdvice(Mentor mentor, Brand brand, AdviceBase advice, bool publish);
        [Obsolete]
        BrandAdvice AddBrandAdvice(int mentorId, int brandId, int semaphoreId, string label, string introduction,
                                   string adviceText, string keywords, bool publish);
        [Obsolete]
        CompanyAdvice AddCompanyAdvice(Mentor mentor, Company company, AdviceBase advice, bool publish);
        [Obsolete]
        CompanyAdvice AddCompanyAdvice(int mentorId, int companyId, int semaphoreId, string label, string introduction,
                                       string adviceText, string keywords, bool publish);
        [Obsolete]
        CountryAdvice AddCountryAdvice(Mentor mentor, Country country, AdviceBase advice, bool publish);
        [Obsolete]
        CountryAdvice AddCountryAdvice(int mentorId, int countryId, int semaphoreId, string label, string introduction,
                                       string adviceText, string keywords, bool publish);
        [Obsolete]
        ConceptAdvice AddConceptAdvice(Mentor mentor, Concept concept, AdviceBase advice, bool publish);
        [Obsolete]
        ConceptAdvice AddConceptAdvice(int mentorId, int conceptId, int semaphoreId, string label, string introduction,
                                       string adviceText, string keywords, bool publish);

        AdviceBase UpdateAdvice(AdviceBase updatedAdvice);
        void PublishAdvice(int adviceId);
        void UnpublishAdvice(int adviceId);
        void DeleteAdvice(int adviceId);
        #endregion

        Mentor GetMentor(int id);
        AdviceBase GetAdvice(int adviceId);

        IList<AdviceTag> GetAllAdviceTags();

        IList<Ingredient> GeIngredientsWithAdvicesByMentor(Mentor mentor);
        IList<Concept> GetConeptsWithAdvicesByMentor(Mentor mentor);
        IList<Company> GetCompaniesWithAdvicesByMentor(Mentor mentor);
        IList<Brand> GetBrandsWithAdvicesByMentor(Mentor mentor);
        IList<Country> GetCountriesWithAdvicesByMentor(Mentor mentor);
        IList<Product> GetProductsWithAdvicesByMentor(Mentor mentor);
    }
}
