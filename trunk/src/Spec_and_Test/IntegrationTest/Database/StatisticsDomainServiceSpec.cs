using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server.Interfaces;
using Consumentor.ShopGun.Repository;
using IntegrationTest.HelperClasses;
using NUnit.Framework;
using Consumentor.ShopGun.DomainService.Server;
using ShopGunSpecBase;
using NBehave.Spec.NUnit;

namespace IntegrationTest.Database
{
    [TestFixture, Category("Integration")]
    public class StatisticsDomainServiceSpec : DatabaseSpecBase
    {
        private IStatisticsDomainService _statisticsDomainService;
        private ProductBuilder _productBuilder;
        private Product _product;
        private User _user;
        private SearchResultMessageEntity _searchResultMessageEntity;

        protected override void Before_all_specs()
        {
            SetupDatabase(ShopGunSpecBase.Database.ShopGun, typeof(Base).Assembly);
            _statisticsDomainService = new StatisticsDomainService(new Repository<Brand>(GetNewDataContext()),
                                                                   new Repository<Company>(GetNewDataContext()),
                                                                   new Repository<Country>(GetNewDataContext()),
                                                                   new Repository<Product>(GetNewDataContext()),
                                                                   new Repository<Ingredient>(GetNewDataContext()),
                                                                   new Repository<Concept>(GetNewDataContext()),
                                                                   new Repository<AdviceBase>(GetNewDataContext()));

            _productBuilder = new ProductBuilder();
            _product = ProductBuilder.BuildProduct();
            _user = UserBuilder.BuildUser();
            using (var dataContext = GetNewDataContext())
            {
                dataContext.GetTable<Product>().InsertOnSubmit(_product);
                dataContext.GetTable<User>().InsertOnSubmit(_user);
                dataContext.SubmitChanges();
            }
            _searchResultMessageEntity = new SearchResultMessageEntity {Products = new List<Product> {_product}};
            base.Before_all_specs();
        }

        protected override void Before_each_spec()
        {
            using (var dataContext = GetNewDataContext() )
            {
                dataContext.GetTable<StatisticsBase>().DeleteAllOnSubmit(dataContext.GetTable<StatisticsBase>().Where(x => x != null));
                dataContext.SubmitChanges();
            }
            base.Before_each_spec();
        }

        [Test]
        public void ShouldAddProductStatisticWithoutSideEffects()
        {
            _statisticsDomainService.AddStatistics(_user, _searchResultMessageEntity, "", "", "", "");

            using (var dataContext = GetNewDataContext())
            {
                var statistics =
                    dataContext.GetTableForType(typeof (ProductStatistic)).OfType<ProductStatistic>().Where(
                        x => x != null);

                statistics.Count().ShouldEqual(1);

                var product = dataContext.GetTable<Product>().Where(x => x != null);
                product.Count().ShouldEqual(1);
                product.ToList()[0].ProductStatistics.Count.ShouldEqual(1);

                var users = dataContext.GetTable<User>().Where(x => x != null);
                users.Count().ShouldEqual(1);
            }
        }
    }
}
