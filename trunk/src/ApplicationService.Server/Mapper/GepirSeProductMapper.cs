using System;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService;
using Consumentor.ShopGun.DomainService.Server.Interfaces;
using Consumentor.ShopGun.Gateway.se.gs1.gepir;

namespace Consumentor.ShopGun.ApplicationService.Server.Mapper
{
    public class GepirSeProductMapper : ProductMapper<itemDataLineType>
    {
        public GepirSeProductMapper(RepositoryFactory repositoryFactory
            , IIngredientApplicationService ingredientApplicationService
            , IBrandApplicationService brandApplicationService
            , ICompanyApplicationService companyApplicationService
            , ICountryApplicationService countryApplicationService)
            : base(repositoryFactory, ingredientApplicationService, brandApplicationService, companyApplicationService, countryApplicationService)
        {
        }

        public override itemDataLineType Map(Product source)
        {
            throw new NotImplementedException();
        }

        public override Product Map(itemDataLineType source)
        {
            var brand = MapBrandByName(source.brandName, false);
            var productName = source.itemName;
            int quantity;
            if(!int.TryParse(source.netContent.Value,out quantity))
            {
                quantity = -1;
            }
            var quantityUnit = source.netContent.uom;
            var mappedProduct = new Product
                                    {
                                        GlobalTradeItemNumber = source.gtin,
                                        Brand = brand,
                                        ProductName = productName,
                                        Quantity = quantity,
                                        QuantityUnit = quantityUnit
                                    };

            return mappedProduct;
        }
    }
}
