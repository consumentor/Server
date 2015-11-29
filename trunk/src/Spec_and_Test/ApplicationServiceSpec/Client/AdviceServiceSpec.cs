using System;
using Castle.Core.Logging;
using Consumentor.ShopGun.ApplicationService;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;
using NBehave.Spec.NUnit;
using Rhino.Mocks;

namespace ApplicationServiceSpec.Client
{
    public abstract class AdviceServiceSpec : SpecBase<IAdviceService>
    {
        private IRepository<AdviceBase> _adviceRepository;
        private RepositoryFactory<IRepository<AdviceBase>, AdviceBase> _repositoryFactory;
        protected override IAdviceService Given_these_conditions()
        {
            _adviceRepository = new InMemoryRepository<AdviceBase>(true);
            _repositoryFactory = CreateStub<RepositoryFactory<IRepository<AdviceBase>, AdviceBase>>();
            _repositoryFactory.Stub(x => x.Build()).Return(_adviceRepository);
            _adviceRepository.Delete(_adviceRepository.Find(r => r != null));


            var sut = new AdviceService<AdviceBase>(_repositoryFactory) { Log = NullLogger.Instance };
            //sut.Log = new ConsoleLogger();

            
            return sut;
        }
    }

    public class AdviceService<TArtefact> : LogBase, IAdviceService
    {
        public AdviceService(RepositoryFactory<IRepository<AdviceBase>, AdviceBase> factory)
        {
            throw new NotImplementedException();
        }
    }

    public interface IAdviceService
    {
    }
}
