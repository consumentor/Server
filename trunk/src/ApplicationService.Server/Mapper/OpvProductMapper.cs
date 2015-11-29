using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService;
using Consumentor.ShopGun.Gateway.Opv;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.ApplicationService.Server.Mapper
{
    [Interceptor(typeof(LogInterceptor))]
    public class OpvProductMapper : ProductMapper<ProductGWO>
    {

        public OpvProductMapper(RepositoryFactory repositoryFactory, IIngredientApplicationService ingredientApplicationService, IBrandApplicationService brandApplicationService, ICompanyApplicationService companyApplicationService, ICountryApplicationService countryApplicationService)
            : base(repositoryFactory, ingredientApplicationService, brandApplicationService, companyApplicationService, countryApplicationService)
        {
        }
        
        public override ProductGWO Map(Product source)
        {
            throw new NotImplementedException();
        }

        public override Product Map(ProductGWO source)
        {
            var brand = MapBrandByName(source.Trademark.Replace("®",""), false);
            if (brand.Owner == null)
            {
                var company = MapCompanyByName(source.Supplier);
                brand.Owner = company;
            }
            var originCountry = MapOriginCountry(source.Origin);

            var allergyInformation = MapAllergyInformation(source);

            var ingredients = IngredientApplicationService.FindIngredientsForTableOfContents(source.Content);

            var markingNames = from marking in source.Markings
                               select marking.MarkName;
            var certificationMarks = MapCertificationMarks(source.Markings);//MapMarkingNames(markingNames);


            var mappedProduct = new Product
            {
                AllergyInformation = allergyInformation,
                CertificationMarks = certificationMarks,
                Brand = brand,
                Description = "",
                GlobalTradeItemNumber = source.EAN,
                Ingredients = ingredients,
                OriginCountry = originCountry,
                ProductName = source.Name,
                //Quantity = 
                QuantityUnit = source.WeightVolume,
                TableOfContent = source.Content,
                LastUpdated = DateTime.Now,
                Durability = source.Durability,
                ImageUrlLarge = source.ImageURL_Jpg300px,
                ImageUrlMedium = source.ImageURL_Jpg150px,
                ImageUrlSmall = source.ImageURL_Jpg66px,
                Nutrition = source.Nutrition,
                SupplierArticleNumber = source.SuppArtNo
            };

            return mappedProduct;
        }

        private IList<AllergyInformation> MapAllergyInformation(ProductGWO source)
        {
            IList<AllergyInformation> allergyInformation = new List<AllergyInformation>();
            foreach (var allergyInfoGwo in source.Allergies)
            {
                var ingredient = IngredientApplicationService.FindIngredient(allergyInfoGwo.SubstanceName, true, true);
                allergyInformation.Add(new AllergyInformation
                                           {
                                               Allergene = ingredient,
                                               Remark = allergyInfoGwo.Remark
                                           });
            }
            return allergyInformation;
        }

        private IList<CertificationMark> MapCertificationMarks(IEnumerable<MarkInfoGWO> opvMarkings)
        {
            using (var opvCertificationMarkMappingsRepository = RepositoryFactory.Build<IRepository<OpvCertificationMarkMapping>, OpvCertificationMarkMapping>())
            {
                var opvCertificationMarkIds = opvMarkings.Select(x => x.MarkId);
                var opvMapppings = from mapping in opvCertificationMarkMappingsRepository.Find(x => x.ProviderCertificationId != null)
                                   where opvCertificationMarkIds.Contains(mapping.ProviderCertificationId.Value)
                                   select mapping;
                var nonMappedMarkings = from marking in opvMarkings
                                        where
                                            !opvMapppings.Select(x => x.ProviderCertificationId).Contains(marking.MarkId)
                                        select marking;
                var result = opvMapppings.Select(x => x.CertificationMark).ToList();

                foreach (var nonMappedMarking in nonMappedMarkings)
                {
                    result.Add(new CertificationMark
                                   {
                                       CertificationName = nonMappedMarking.MarkName                                       
                                   });
                }

                return result;
            }
        }

        private IList<CertificationMark> MapMarkingNames(IEnumerable<string> markingNames)
        {
            using (
                var certificationMarkRepository =
                    RepositoryFactory.Build<IRepository<CertificationMark>, CertificationMark>())
            {
                IList<CertificationMark> certificationMarks = new List<CertificationMark>(markingNames.Count());
                foreach (var markingName in markingNames)
                {
                    var name = markingName;
                    CertificationMark certificationMark;
                    try
                    {
                        certificationMark =
                            certificationMarkRepository.FindOne(m => m.CertificationName == name);
                    }
                    catch (Exception)
                    {
                        Log.Debug("CertificationMark not found when mapping: {0}", name);
                        certificationMark = new CertificationMark { CertificationName = name };
                    }
                    certificationMarks.Add(certificationMark);
                }
                return certificationMarks;
            }
        }
    }
}
