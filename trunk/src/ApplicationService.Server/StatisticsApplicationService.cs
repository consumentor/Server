using System;
using System.Collections.Generic;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.DomainService.Server;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server.Interfaces;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    public class StatisticsApplicationService : IStatisticsApplicationService
    {
        private readonly IStatisticsDomainService _statisticsDomainService;
        private readonly ISearchStatisticsDomainService _searchStatisticsDomainService;

        public StatisticsApplicationService(IStatisticsDomainService statisticsDomainService, ISearchStatisticsDomainService searchStatisticsDomainService)
        {
            _statisticsDomainService = statisticsDomainService;
            _searchStatisticsDomainService = searchStatisticsDomainService;
        }

        #region IStatisticsApplicationService Members

        public IDictionary<AdviceBase, IList<StatisticsBase>> GetAdviceStatistics(Platforms platform, DateTime fromDate, DateTime untilDate)
        {
            throw new NotImplementedException();
        }

        public Searchterm GetSearchStatistics(string searchstring)
        {
            return _searchStatisticsDomainService.GetSearchStatistics(searchstring);
        }

        public IList<Searchterm> GetSearchterms()
        {
            return _searchStatisticsDomainService.GetSearchterms();
        }

        public IList<Searchterm> GetTopNSearchterms(int n)
        {
            return _searchStatisticsDomainService.GetTopNSearchterms(n);
        }

        #endregion
    }
}
