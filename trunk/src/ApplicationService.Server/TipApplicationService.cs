using System;
using System.Linq;
using Castle.Core;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    [Interceptor(typeof(LogInterceptor))]
    public class TipApplicationService : ITipApplicationService
    {
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IRepository<Tip> _tipRepository;

        public TipApplicationService(RepositoryFactory repositoryFactory, IRepository<Tip> tipRepository)
        {
            _repositoryFactory = repositoryFactory;
            _tipRepository = tipRepository;
        }

        public Tip GetRandomTip()
        {
            var randomizer = new Random(DateTime.Now.Millisecond);
            var choice = randomizer.Next(0, _tipRepository.Find(x => x.Published).Count()-1);
            return _tipRepository.Find(x => x != null).Skip(choice).FirstOrDefault();
        }
    }
}
