using System;
using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server.Interfaces
{
    public interface IStatisticsApplicationService
    {
        IDictionary<AdviceBase, IList<StatisticsBase>> GetAdviceStatistics(Platforms platform, DateTime fromDate, DateTime untilDate);
        Searchterm GetSearchStatistics(string searchstring);
        IList<Searchterm> GetSearchterms();
        IList<Searchterm> GetTopNSearchterms(int n);
    }
}
