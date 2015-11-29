using System.IO;
using Consumentor.ShopGun.ApplicationService;
using Consumentor.ShopGun.ApplicationService.Server;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.ApplicationService.Server.Mapper;
using Consumentor.ShopGun.Component;
using Consumentor.ShopGun.Configuration;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService;
using Consumentor.ShopGun.DomainService.Repository;
using Consumentor.ShopGun.DomainService.Server;
using Consumentor.ShopGun.DomainService.Server.ExternalInformationProvider;
using Consumentor.ShopGun.DomainService.Server.Interfaces;
using Consumentor.ShopGun.Gateway;
using Consumentor.ShopGun.Gateway.Opv;
using Consumentor.ShopGun.Gateway.se.gs1.gepir;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;
using Consumentor.Shopgun.Web.Community.UI.Controllers;

namespace Consumentor.Shopgun.Web.Community.UI
{
    public class IocConfig : IContainerConfiguration
    {
        private readonly IContainer _container;

        public IocConfig(IContainer container)
        {
            _container = container;
        }

        void IContainerConfiguration.Setup()
        {
            ApplicationServices();
            DomainServices();
            Log();
            Gateways();
            Infrastructure();
            Mappers();
            //WebServices();
            Controllers();
            //ViewComponents();
        }

        private void ApplicationServices()
        {
            _container.RegisterComponent()
                .AsTransient
                .AsService(typeof(RepositoryFactory))
                .OfType(typeof(RepositoryFactory));

            _container.RegisterComponent()
            .AsTransient
            .AsService(typeof(RepositoryFactory<,>))
            .OfType(typeof(RepositoryFactory<,>));

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IIngredientApplicationService>()
                .OfType<IngredientApplicationService>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IConceptApplicationService>()
                .OfType<ConceptApplicationService>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<ICompanyApplicationService>()
                .OfType<CompanyApplicationService>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IProductApplicationService>()
                .OfType<ProductApplicationService>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<ICountryApplicationService>()
                .OfType<CountryApplicationService>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService(typeof(IAdviceApplicationService))
                .OfType(typeof(AdviceApplicationService));

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IMembershipProviderApplicationService>()
                .OfType<MembershipProviderApplicationService>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IRoleProviderApplicationService>()
                .OfType<RoleProviderApplicationService>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IStatisticsApplicationService>()
                .OfType<StatisticsApplicationService>();

            _container.RegisterComponent()
               .AsSingleton
               .AsService<IBrandApplicationService>()
               .OfType<BrandApplicationService>();

            _container.RegisterComponent()
               .AsSingleton
               .AsService<IMentorApplicationService>()
               .OfType<MentorApplicationService>();

            _container.RegisterComponent()
               .AsSingleton
               .AsService<ISemaphoreApplicationService>()
               .OfType<SemaphoreApplicationService>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IOpvProductInformationDomainService>()
                .OfType<OpvProductInformationDomainService>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IGepirProductInformationDomainService>()
                .OfType<GepirProductInformationDomainService>();


            _container.RegisterComponent()
                .AsSingleton
                .AsService<IAdviceSearchApplicationService>()
                .OfType<AdviceSearchApplicationService>();
        }

        private void DomainServices()
        {
            _container.RegisterComponent()
                .AsSingleton
                .AsService<ISearchStatisticsDomainService>()
                .OfType<SearchStatisticsDomainService>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IStatisticsDomainService>()
                .OfType<StatisticsDomainService>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IMentorDomainService>()
                .OfType<MentorDomainService>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<ISemaphoreDomainService>()
                .OfType<SemaphoreDomainService>();

            _container.RegisterComponent()
                .AsTransient
                .AsService<IProductRepository>()
                .OfType<ProductRepository>();

            _container.RegisterComponent()
                .AsTransient
                .AsService<IIngredientRepository>()
                .OfType<IngredientRepository>();

            _container.RegisterComponent()
                .AsTransient
                .AsService<IBrandRepository>()
                .OfType<BrandRepository>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<ICategoryInfoDomainService>()
                .OfType<CategoryInfoDomainService>();

            _container.RegisterComponent()
               .AsSingleton
               .AsService<ICertificationMarkDomainService>()
               .OfType<CertificationMarkDomainService>();

        }

        private void Gateways()
        {
            _container.RegisterComponent()
                .AsSingleton
                .AsService<IOpvWebServiceFactory>()
                .OfType<OpvWebServiceFactory>();

            _container.RegisterComponent()
                .AsTransient
                .AsService<ProductSearchWebServiceGateway>()
                .OfType<ProductSearchWebServiceGateway>();
        }

        private void Infrastructure()
        {
            _container.RegisterComponent()
            .AsTransient
            .AsService(typeof(IAdviceRepository))
            .OfType(typeof(AdviceRepository));

            _container.RegisterComponent()
                .AsTransient
                .AsService(typeof(IRepository<>))
                .OfType(typeof(Repository<>));

            _container.RegisterComponent()
                .AsSingleton
                .AsService(typeof(ISqlDependency))
                .OfType(typeof(SqlDependency));

            _container.RegisterComponent()
                .AsSingleton
                .AsService(typeof(IConfiguration))
                .OfType(typeof(BasicConfiguration));

            _container.RegisterComponent()
                .AsTransient
                .AsService(typeof(DataContext))
                .OfType(typeof(DataContext));

            _container.RegisterComponent()
                .AsSingleton
                .AsService(typeof(IAttributeMappingSource))
                .OfType(typeof(AttributeMappingSource));


        }

        private void Log()
        {
            _container.RegisterComponent()
                .AsTransient
                .AsService(typeof(LogInterceptor))
                .OfType(typeof(LogInterceptor));

            _container.RegisterComponent()
                .AsTransient
                .AsService(typeof(TextWriter))
                .OfType(typeof(DataContextLog));
        }

        private void Mappers()
        {
            _container.RegisterComponent()
                .AsSingleton
                .AsService<IMapper<Product, ProductGWO>>()
                .OfType<OpvProductMapper>();


            _container.RegisterComponent()
                .AsSingleton
                .AsService<IMapper<Product, itemDataLineType>>()
                .OfType<GepirSeProductMapper>();


            _container.RegisterComponent()
                .AsSingleton
                .AsService<IMapper<Company, partyDataLineType>>()
                .OfType<GepirCompanyMapper>();


        }

        private void WebServices()
        {

        }


        private void Controllers()
        {
            //_container.RegisterComponent().AsTransient.WithKey("BreadcrumbComponent.ViewComponent").OfType<BreadcrumbComponent>();
            _container.RegisterComponent("Home", typeof(HomeController), ComponentLifestyle.Transient);
            _container.RegisterComponent("Account", typeof(AccountController), ComponentLifestyle.Transient);
        }

        private void ViewComponents()
        {
           
        }
    }
}