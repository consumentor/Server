using System.IO;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.ApplicationService.Server.Mapper;
using Consumentor.ShopGun.Component;
using Consumentor.ShopGun.Configuration;
using Consumentor.ShopGun.Context;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Domain.DataTransferObject;
using Consumentor.ShopGun.DomainService;
using Consumentor.ShopGun.DomainService.DtoMapper;
using Consumentor.ShopGun.DomainService.Repository;
using Consumentor.ShopGun.DomainService.Server;
using Consumentor.ShopGun.DomainService.Server.ExternalInformationProvider;
using Consumentor.ShopGun.DomainService.Server.Interfaces;
using Consumentor.ShopGun.Gateway;
using Consumentor.ShopGun.Gateway.Opv;
using Consumentor.ShopGun.Gateway.se.gs1.gepir;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;
using Consumentor.ShopGun.Services;


namespace Consumentor.ShopGun.ApplicationService.Server
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
            Log();
            ApplicationServices();
            Gateways();
            Infrastructure();
            Mappers();
            DomainServices();
            WebServices();
        }

        private void ApplicationServices()
        {
            _container.RegisterComponent()
                .AsSingleton
                .AsService<ITipApplicationService>()
                .OfType<TipApplicationService>();


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
                .AsService<IUserRegistrationApplicationService>()
                .OfType<UserRegistrationApplicationService>();

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
                .AsService<IProductApplicationService>()
                .OfType<ProductApplicationService>();

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
               .AsService<IBrandApplicationService>()
               .OfType<BrandApplicationService>();

            _container.RegisterComponent()
               .AsSingleton
               .AsService<ICompanyApplicationService>()
               .OfType<CompanyApplicationService>();

            _container.RegisterComponent()
               .AsSingleton
               .AsService<ICountryApplicationService>()
               .OfType<CountryApplicationService>();

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
                .AsService<IExternalProductInformationProviderApplicationService>()
                .OfType<ExternalProductInformationProviderApplicationService>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IAdviceSearchApplicationService>()
                .OfType<AdviceSearchApplicationService>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IOpvProductInformationDomainService>()
                .OfType<OpvProductInformationDomainService>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IGepirProductInformationDomainService>()
                .OfType<GepirProductInformationDomainService>();
            
            //_container.RegisterComponent()
            //    .AsSingleton
            //    .AsService<IRollMapApplicationService>()
            //    .OfType<RollMapApplicationService>();

            //_container.RegisterComponent()
            //    .AsTransient
            //    .AsService<IRollMapRouterApplicationService>()
            //    .OfType<RollMapRouterApplicationService>();

            //Publishers();
        }

        private void Publishers()
        {
            //_container.RegisterComponent()
            //    .AsSingleton
            //    .WithKey(typeof(FaultCodeListPublisher).Name)
            //    //.AsService<FaultCodeListPublisher>() //TODO: IPublisher???
            //    .AsService<IPublisher>()
            //    .OfType<FaultCodeListPublisher>();

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .WithKey(typeof(SlitterRollMapPublisher).Name)
            //    .AsService<IPublisher>()
            //    .OfType<SlitterRollMapPublisher>();

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .WithKey(typeof(DoctoringRollMapPublisher).Name)
            //    .AsService<IPublisher>()
            //    .OfType<DoctoringRollMapPublisher>();

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .WithKey(typeof(PrintOrder).Name)
            //    .AsService<IPublisher>()
            //    .OfType(typeof(OrderPublisher<PrintOrder>));

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .WithKey(typeof(LaminatorOrder).Name)
            //    .AsService<IPublisher>()
            //    .OfType(typeof(OrderPublisher<LaminatorOrder>));

            //_container.RegisterComponent()
            //   .AsSingleton
            //   .WithKey(typeof(SlitterOrder).Name)
            //   .AsService<IPublisher>()
            //    .OfType(typeof(OrderPublisher<SlitterOrder>));

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .WithKey(typeof(DoctoringOrder).Name)
            //    .AsService<IPublisher>()
            //    .OfType(typeof(OrderPublisher<DoctoringOrder>));

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

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .AsService<ICallbackWebServiceSubscriberFactory>()
            //    .OfType<CallBackWebServiceSubscriberFactory>();

            //_container.RegisterComponent()
            //    .AsTransient
            //    .AsService<IP2RollMapWebServiceGateway>()
            //    .OfType<P2RollMapWebServiceGateway>();
        }

        private void Infrastructure()
        {
            _container.RegisterComponent()
                .AsSingleton
                .AsService<IServiceCultureConfiguration>()
                .OfType<ServiceCultureConfiguration>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IWebServiceConfiguration>()
                .OfType<WebServiceConfiguration>();

            _container.RegisterComponent()
                .AsTransient
                .AsService<IActionPerformer>()
                .OfType<ActionPerformer>();

            _container.RegisterComponent()
                .AsTransient
                .AsService(typeof(WinServiceHost<>))
                .OfType(typeof(WinServiceHost<>));

            _container.RegisterComponent()
                .AsTransient
                .AsService(typeof(WebServiceHost<>))
                .OfType(typeof(WebServiceHost<>));

            _container.RegisterComponent()
            .AsSingleton
            .AsService(typeof(IAdviceRepository))
            .OfType(typeof(AdviceRepository));

            _container.RegisterComponent()
                .AsTransient
                .AsService(typeof(IRepository<>))
                .OfType(typeof(Repository<>));

            _container.RegisterComponent()
                .AsTransient
                .AsService<DataContext>()
                .OfType<DataContext>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IAttributeMappingSource>()
                .OfType<AttributeMappingSource>();

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IConfiguration>()
                .OfType<BasicConfiguration>();

            _container.RegisterComponent()
               .AsTransient
               .AsService<IShopgunWebOperationContext>()
               .OfType<ShopgunWebOperationContext>();

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .AsService(typeof(IWebServicePublisherConfiguration<>))
            //    .OfType(typeof(WebServicePublisherConfiguration<>));

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .AsService<SubscriberConfigurationFactory>()
            //    .OfType<SubscriberConfigurationFactory>();

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .WithKey(typeof(DefaultSubscriberConfiguration).Name)
            //    .AsService<ISubscriberConfiguration>()
            //    .OfType<DefaultSubscriberConfiguration>();

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .AsService<SchedulerConfigurationFactory>()
            //    .OfType<SchedulerConfigurationFactory>();

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .WithKey(typeof(DefaultSchedulerConfiguration).Name)
            //    .AsService<ISchedulerConfiguration>()
            //    .OfType<DefaultSchedulerConfiguration>();
        }

        private void Log()
        {
            _container.RegisterComponent()
                .AsTransient
                .AsService<LogInterceptor>()
                .OfType<LogInterceptor>();

            _container.RegisterComponent()
                .AsTransient
                .AsService<TextWriter>()
                .OfType<DataContextLog>();
        }

        private void Mappers()
        {
            //_container.RegisterComponent()
            //    .AsSingleton
            //    .AsService(typeof(IListMapper<,>))
            //    .OfType(typeof(ListMapper<,>));

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

            _container.RegisterComponent()
                .AsSingleton
                .AsService<IMapper<Mentor, MentorDto>>()
                .OfType<MentorDtoMapper>();

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .AsService(typeof(IListMapper<,>))
            //    .OfType(typeof(ListMapper<,>));

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .AsService<IMapper<WebServiceCallbackArguments, WebServiceCallbackArgumentsGWO>>()
            //    .OfType<WebServiceCallbackEventArgsMapper>();

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .AsService<IMapper<FaultCodeList, Domain.FaultCodeList>>()
            //    .OfType<FaultCodeListMapper>();


            //_container.RegisterComponent()
            //    .AsSingleton
            //    .WithKey("P2Simulator.RollMapMapper")
            //    .AsService<IMapper<RollMap, RollMapGWO>>()
            //    .OfType<Mapper.P2Simulator.RollMapMapper>();

            //    _container.RegisterComponent()
            //    .AsSingleton
            //    .WithKey("Server.RollMapMapper")
            //    .AsService<IMapper<RollMap, Gateway.Server.RollMapGWO>>()
            //    .OfType<RollMapMapper>();

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

            //_container.RegisterComponent()                
            //    .AsSingleton
            //    .AsService<IFaultCodeDomainService>()
            //    .OfType<FaultCodeDomainService>();

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .AsService(typeof(IOrderDomainService<>))
            //    .OfType(typeof(OrderDomainService<>));

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .AsService<IRollMapDomainService>()
            //    .OfType<RollMapDomainService>();

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .AsService<IDefectImageDomainService>()
            //    .OfType<DefectImageDomainService>();

            //_container.RegisterComponent()
            //    .AsSingleton
            //    .AsService<IClusterDefectDomainService>()
            //    .OfType<ClusterDefectDomainService>();
        }

        private void WebServices()
        {
        }
    }
}
