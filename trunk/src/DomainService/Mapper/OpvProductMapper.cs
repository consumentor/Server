using System;
using System.Collections.Generic;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Gateway.Opv;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.DomainService.Mapper
{
    //[Obsolete]
    public class OpvProductMapper : ProductMapper<ProductGWO>
    {
        public OpvProductMapper(RepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public override ProductGWO Map(Product source)
        {
            throw new NotImplementedException();
        }

        public override Product Map(ProductGWO source)
        {
            var company = MapCompany(source.Supplier);
            var brand = MapBrand(source.Trademark);
            if (brand.Owner == null)
            {
                brand.Owner = company;
            }
            var ingredients = MapTableOfContentsToIngredients(source.Content);
            var originCountry = MapOriginCountry(source.Origin);

            IList<AllergyInformation> allergyInformation = new List<AllergyInformation>();
            foreach (var allergyInfoGwo in source.Allergies)
            {
                var ingredient = MapIngredient(allergyInfoGwo.SubstanceName);
                allergyInformation.Add(new AllergyInformation
                                           {
                                               Allergene = ingredient,
                                               Remark = allergyInfoGwo.Remark
                                           });
            }

            var certificationMarks = new List<CertificationMark>();
            using (
                var certificationMarkRepository =
                    RepositoryFactory.Build<IRepository<CertificationMark>, CertificationMark>())
            {
                foreach (var markInfoGwo in source.Markings)
                {
                    var markInfoGwoCopy = markInfoGwo;
                    CertificationMark certificationMark;
                    try
                    {
                        certificationMark =
                            certificationMarkRepository.FindOne(m => m.CertificationName == markInfoGwoCopy.MarkName);
                    }
                    catch (Exception)
                    {
                        certificationMark = new CertificationMark {CertificationName = markInfoGwo.MarkName};
                    }
                    certificationMarks.Add(certificationMark);
                }
            }

            return new Product
                       {
                           AllergyInformation = allergyInformation,
                           Brand = brand,
                           Description = "",
                           GlobalTradeItemNumber = source.EAN,
                           //Ingredients = ingredients,
                           //CertificationMarks = certificationMarks,
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
        }
    }
}
